import jwtDecode from 'jwt-decode';
import { useCallback, useEffect, useRef, useState } from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';

import { PageLoader } from '../PageLoader';

import { LogOutPopUp } from '@/components/LogOutPopUp';
import {
	INACTIVE_LIMIT,
	LOGOUT_REASON,
	LOG_OUT_STATUS,
	LogoutReasons,
	LogoutStatuses,
	REDIRECT_AFTER_LOGIN_URL,
	SESSION_DURATION,
	SESSION_EXPIRES,
	SHOWING_LOG_OUT_POPUP_DURATION,
	USER_ACTIVITY_EVENTS,
	USER_LAST_ACTIVITY_DATE,
} from '@/constants/authConstants';
import { timeUnits } from '@/constants/date';
import {
	NotificationType,
	SESSION_EXPIRED as SESSION_EXPIRED_TOAST,
} from '@/constants/notifications';
import { Routes } from '@/constants/routes';
import { logoutInOtherTabs, useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { checkIsShowSetProfile } from '@/helpers/checkIsShowSetProfile';
import { createToast } from '@/helpers/createToast';
import { setAuthState } from '@/helpers/localStorage';
import { useAxiosInterCeptors } from '@/hooks/useAxiosInterceptors';
import { useConfigurationContextValue } from '@/hooks/useConfigurationContextValue';
import { useUserInfo } from '@/queries/useUserInfo';
import { DecodedToken } from '@/types/common';

import { Timeout } from './types';
import { extractClaims } from './utils';

const { SESSION_EXPIRED, INACTIVITY } = LogoutReasons;

// Variable containing the date of the last user activity
let lastUserActivityDate = Date.now();
// Function to notify the current and other tabs in the application that the user was active
function notifyCurrentAndOtherTabsUserWasActive() {
	// Update last activity time
	lastUserActivityDate = Date.now();
	// Record the time of last activity in localStorage
	localStorage.setItem(
		USER_LAST_ACTIVITY_DATE,
		lastUserActivityDate.toString()
	);
}

// Function to logout on the current tab
function logoutInCurrentTab(reason: string, logoutFn: () => void) {
	// Call the logout function

	logoutFn();
}

// Variable that stores Timeout, which is responsible for the time of inactivity
let inactivityInterval: Timeout;
let sessionActivityInterval: Timeout;

// Callback function to respond to changes in storage
function handleStorageEvent(event: StorageEvent) {
	if (event.key === LOGOUT_REASON) {
		// Call the global function for logout if there is a reason for this in localStorage
		window.logout();
	} else if (event.key === USER_LAST_ACTIVITY_DATE) {
		if (event.newValue) {
			lastUserActivityDate = +new Date(+event.newValue);
		}
	} else if (event.key === LOG_OUT_STATUS) {
		if (event.newValue === LogoutStatuses.STAY_LOG_IN) {
			window.cancelLogout();
		}
	}
}

// Function to stop tracking user inactivity
export function destroyLogout() {
	// Disable interval
	clearInterval(inactivityInterval);
	clearInterval(sessionActivityInterval);
	window.removeEventListener('storage', handleStorageEvent);
	USER_ACTIVITY_EVENTS.forEach((event) => {
		window.removeEventListener(event, notifyCurrentAndOtherTabsUserWasActive);
	});
}

/**
 * Initializing the logout system
 */
function initLogout(
	logoutFn: () => void,
	showLogoutPopUpFn: () => void,
	cancelLogoutFn: () => void
) {
	/**
	 * We call this function during initialization in order to handle the situation,
	 * when the user opens the page and does not interact with it.
	 * In this case, we still need to start tracking inactivity
	 */
	notifyCurrentAndOtherTabsUserWasActive();

	// Clear the logout reason
	localStorage.setItem(LOGOUT_REASON, '');

	// Add a logout method to the global window object to exit the application
	window.logout = (reason = INACTIVITY) => {
		destroyLogout();
		logoutInOtherTabs(reason);
		logoutInCurrentTab(reason, logoutFn);
	};

	window.cancelLogout = () => {
		cancelLogoutFn();
	};

	// Set the interval to track user inactivity
	inactivityInterval = setInterval(() => {
		if (Date.now() - lastUserActivityDate > INACTIVE_LIMIT) {
			showLogoutPopUpFn();
			localStorage.setItem(LOG_OUT_STATUS, LogoutStatuses.LOG_OUT);
		}
	}, 1000);

	// Add a change listener to storage
	window.addEventListener('storage', handleStorageEvent);

	// Add listeners to all user activity events
	USER_ACTIVITY_EVENTS.forEach((event) => {
		window.addEventListener(event, notifyCurrentAndOtherTabsUserWasActive);
	});
}

/**
 * Hide routes if user doesn't have token and data of current user
 */
export function ProtectedRoute() {
	const {
		role,
		token,
		onLogout,
		setUserInfo,
		isLoading: isLoadingAuth,
		location: userLocation,
		departmentId,
		position,
	} = useAuth();
	const { isLoading: isLoadingConfiguration } = useConfiguration();

	const location = useLocation();
	const [isLogOutDialogOpen, setIsLogOutDialogOpen] = useState(false);
	const logOutRef = useRef<Timeout>(undefined);
	const toast = useToast();

	const handleOpenLogOutDialog = useCallback(
		() => setIsLogOutDialogOpen(true),
		[]
	);
	const handleCloseLogOutDialog = useCallback(
		() => setIsLogOutDialogOpen(false),
		[]
	);

	const onCancelLogout = () => {
		handleCloseLogOutDialog();
		clearTimeout(logOutRef.current);
	};

	const handleStayLogIn = useCallback(() => {
		notifyCurrentAndOtherTabsUserWasActive();
		onCancelLogout();
		localStorage.setItem(LOG_OUT_STATUS, LogoutStatuses.STAY_LOG_IN);
	}, []);

	const handleLogOut = useCallback(() => {
		onLogout(INACTIVITY);
	}, []);

	useAxiosInterCeptors(token, onLogout);

	const { isLoading } = useUserInfo(setAuthState, {
		isChangedOnlyTimeZone: false,
		enabled: Boolean(token),
		onSuccess: (userInfoData) => {
			setUserInfo((prev) => ({
				...prev,
				...userInfoData,
			}));
		},
	});

	useConfigurationContextValue(token);

	useEffect(() => {
		if (isLogOutDialogOpen) {
			logOutRef.current = setTimeout(() => {
				onLogout(INACTIVITY);
				handleCloseLogOutDialog();
			}, SHOWING_LOG_OUT_POPUP_DURATION);

			return () => {
				clearTimeout(logOutRef.current);
			};
		}
	}, [isLogOutDialogOpen]);

	useEffect(() => {
		if (token) {
			const decodedToken = jwtDecode<DecodedToken>(token);
			const claims = extractClaims(decodedToken);

			setUserInfo((prev) => ({
				...prev,
				claims,
			}));

			const getSessionExpires = () => {
				if (decodedToken?.exp) {
					return decodedToken.exp * timeUnits.MILLISECONDS_IN_SECOND;
				}

				return Date.now() + SESSION_DURATION;
			};

			sessionStorage.setItem(SESSION_EXPIRES, String(getSessionExpires()));
			initLogout(onLogout, handleOpenLogOutDialog, onCancelLogout);
		}

		return () => {
			destroyLogout();
		};
	}, [token]);

	useEffect(() => {
		if (token) {
			sessionActivityInterval = setInterval(() => {
				const sessionExpireTime = Number(
					sessionStorage.getItem(SESSION_EXPIRES) ?? 0
				);

				if (sessionExpireTime && Date.now() > sessionExpireTime) {
					onLogout(SESSION_EXPIRED);

					toast.open(createToast(NotificationType.INFO, SESSION_EXPIRED_TOAST));
				}
			}, 1000);
		}

		return () => clearInterval(sessionActivityInterval);
	}, [token]);

	const isLoadingDependencies =
		isLoading || isLoadingAuth || isLoadingConfiguration;
	const isNavigateToSignIn = !token;

	const canShowContent =
		!isLoadingDependencies &&
		token &&
		role &&
		userLocation !== undefined &&
		departmentId !== undefined &&
		position !== undefined;

	const isNavigateToSetProfile =
		canShowContent &&
		checkIsShowSetProfile(userLocation, departmentId, position);

	if (isLoadingDependencies) {
		return <PageLoader />;
	}

	if (isNavigateToSetProfile) {
		return (
			<Navigate to={Routes.setProfile} replace state={{ from: location }} />
		);
	}

	if (isNavigateToSignIn) {
		const redirectUrl = location.pathname + location.search;

		localStorage.setItem(REDIRECT_AFTER_LOGIN_URL, redirectUrl);

		return <Navigate to={Routes.signIn} replace state={{ from: location }} />;
	}

	if (!canShowContent) {
		return <PageLoader />;
	}

	return (
		<>
			<Outlet />{' '}
			{isLogOutDialogOpen && (
				<LogOutPopUp
					handleStayLogIn={handleStayLogIn}
					handleLogOut={handleLogOut}
				/>
			)}
		</>
	);
}

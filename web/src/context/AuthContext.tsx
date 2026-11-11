import {
	PropsWithChildren,
	createContext,
	useCallback,
	useContext,
	useMemo,
	useState,
	useRef,
} from 'react';
import { useNavigate } from 'react-router-dom';

import { destroyLogout } from '@/components/ProtectedRoute';
import {
	LOGOUT_MESSAGE,
	LOGOUT_REASON,
	LogoutReasons,
} from '@/constants/authConstants';
import { Routes } from '@/constants/routes';
import {
	getAuthState,
	initialAuthState,
	removeAuthStateFromStorage,
	setAuthState,
} from '@/helpers/localStorage';
import { useBroadcastChannel } from '@/hooks/useBroadcastChannel';
import { useLogout } from '@/queries/useLogout';
import { UserInfo } from '@/types/userInfo';

import {
	AuthContextType,
	HandleReceivedToken,
	InitialAuthState,
} from './types';

const { VIA_BUTTON } = LogoutReasons;

// Function to notify inactive tabs that it is necessary to logout
export function logoutInOtherTabs(reason: string, message?: string) {
	// Set the logout reason to localStorage
	localStorage.setItem(LOGOUT_REASON, reason);

	if (message) {
		localStorage.setItem(LOGOUT_MESSAGE, message);
	}
}

const AuthContext = createContext<AuthContextType>(initialAuthState);

function AuthProvider({ children }: PropsWithChildren) {
	const { refetch: logoutRequest } = useLogout();
	const [authorization, setAuthorization] = useState(getAuthState());
	const [userInfo, setUserInfo] = useState<Partial<UserInfo>>({});
	const [isLoading, setIsLoading] = useState<boolean>(true);
	const logoutCallbackRef = useRef<VoidFunction | null>(null);
	const navigate = useNavigate();

	useBroadcastChannel(setIsLoading, setAuthorization);

	const handleReceivedToken = useCallback<HandleReceivedToken>(
		(newToken) => {
			const newState: InitialAuthState = {
				...authorization,
				token: newToken,
			};
			setAuthorization(newState);
			setAuthState(newState);
		},
		[authorization]
	);

	const onLogout = (reason?: LogoutReasons, message?: string) => {
		logoutRequest();
		logoutCallbackRef.current?.();
		removeAuthStateFromStorage();
		setAuthorization(initialAuthState);
		logoutInOtherTabs(reason ?? VIA_BUTTON, message);
		destroyLogout();
		navigate(Routes.signIn);
	};

	const value = useMemo<AuthContextType>(
		() => ({
			...authorization,
			...userInfo,
			role: userInfo?.position ?? authorization.role,
			handleReceivedToken,
			onLogout,
			setUserInfo,
			isLoading,
			isUserInfoLoaded: Boolean(userInfo.id),
			setLogoutCallback: (func) => {
				logoutCallbackRef.current = func;
			},
		}),
		[authorization, userInfo, isLoading]
	);

	return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

/**
 * Hook to use AuthContext.
 * Includes data about current user, authorization events and loading status.
 */
const useAuth = () => useContext(AuthContext) as Required<AuthContextType>;

export { AuthContext, AuthProvider, useAuth };

import { PropsWithChildren } from 'react';
import { useLocation } from 'react-router-dom';

import { Routes } from '@/constants/routes';
import { useAuth } from '@/context/AuthContext';

import {
	useCleanDeniedParams,
	useDeniedPopUpNotifications,
	usePopUpAccessCheck,
	usePopUpRenderer,
	useRedirectHome,
} from './hooks';

export function PopUpUrlInterceptor({ children }: PropsWithChildren) {
	const { pathname, search } = useLocation();

	const { isLoading: isAuthLoading, id: userId, isUserInfoLoaded } = useAuth();

	const isRedirect = pathname === Routes.all;
	const isUserInitialized = Boolean(
		userId && !isAuthLoading && isUserInfoLoaded
	);

	const {
		allowed: allowedPopUps,
		denied: deniedPopUps,
		valueDenied: valueDeniedPopUps,
	} = usePopUpAccessCheck();

	useRedirectHome({ isRedirect, allowedPopUps, search });

	useDeniedPopUpNotifications({
		valueDeniedPopUps,
		isUserInitialized,
	});

	useCleanDeniedParams({ deniedPopUps, isUserInitialized });

	const popUps = usePopUpRenderer({
		allowedPopUps,
		isUserInitialized,
	});

	return (
		<>
			{children}
			{popUps}
		</>
	);
}

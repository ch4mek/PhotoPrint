import { useEffect, useRef } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

import { REDIRECT_AFTER_LOGIN_URL } from '@/constants/authConstants';
import {
	AUTHORIZATION_ERROR_MESSAGE,
	NotificationType,
} from '@/constants/notifications';
import { Routes } from '@/constants/routes';
import { AUTH_STORAGE_KEY } from '@/constants/storageKeys';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { useStickyState } from '@/hooks/useStickyState';
/**
 * Page the user will be taken to after logging in
 */
export function AuthCallbackPage() {
	const location = useLocation();
	const navigate = useNavigate();
	const queryParams = new URLSearchParams(location.search);
	const [prevSessionAuthData] = useStickyState<{ token: string }>(
		{ token: '' },
		AUTH_STORAGE_KEY
	);
	const { handleReceivedToken, role, isLoading } = useAuth();
	const toast = useToast();
	const hasRedirected = useRef(false);

	useEffect(() => {
		const jwtToken = queryParams.get('token');

		if (!jwtToken && !prevSessionAuthData.token) {
			const error = queryParams.get('error');
			navigate(Routes.signIn);
			toast.open(
				createToast(
					NotificationType.ERROR,
					error ?? AUTHORIZATION_ERROR_MESSAGE
				)
			);

			return;
		}

		handleReceivedToken(jwtToken ?? prevSessionAuthData.token);
	}, []);

	useEffect(() => {
		const shouldRedirect = role && !isLoading && !hasRedirected.current;

		if (shouldRedirect) {
			hasRedirected.current = true;

			const redirectAfterLogin = localStorage.getItem(REDIRECT_AFTER_LOGIN_URL);

			if (redirectAfterLogin) {
				localStorage.removeItem(REDIRECT_AFTER_LOGIN_URL);
				navigate(redirectAfterLogin, {
					replace: true,
					state: { redirectAfterLogin },
				});
			} else {
				navigate(Routes.root, { replace: true });
			}
		}
	}, [role, isLoading]);

	return null;
}

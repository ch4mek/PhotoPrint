import { SignIn } from '@mms/mms-ui-library/components/SignIn';
import { useEffect, useState } from 'react';
import { Navigate, useLocation } from 'react-router-dom';

import { LogoutMessagePopUp } from '@/components/LogoutMessagePopup';
import { LOGOUT_MESSAGE } from '@/constants/authConstants';
import { API_URL, MICROSOFT_LOGIN_REDIRECT } from '@/constants/queryPaths';
import { DEFAULT_NAVIGATION_ROUTE } from '@/constants/routes';
import { useAuth } from '@/context/AuthContext';

export function SignInPage() {
	const { token } = useAuth();
	const location = useLocation();
	const [isLogOutPopupOpen, setIsLogOutPopupOpen] = useState(false);
	const errorMessage = localStorage.getItem(LOGOUT_MESSAGE);

	useEffect(() => {
		if (errorMessage) {
			setIsLogOutPopupOpen(true);
		}
	}, [errorMessage]);

	const login = () => {
		const origin = window.location.origin;
		window.location.href = `${API_URL}${MICROSOFT_LOGIN_REDIRECT}?returnUrl=${origin}/auth-callback`;
	};

	const handleCloseMessagePopup = () => {
		setIsLogOutPopupOpen(false);
		localStorage.removeItem(LOGOUT_MESSAGE);
	};

	if (token) {
		return (
			<Navigate
				to={location.state?.from.pathname ?? DEFAULT_NAVIGATION_ROUTE}
				replace
			/>
		);
	}

	return (
		<>
			<SignIn onClick={login} />

			{isLogOutPopupOpen && errorMessage && (
				<LogoutMessagePopUp
					handleClose={handleCloseMessagePopup}
					message={errorMessage}
				/>
			)}
		</>
	);
}

import { Box, Button, TextField, Typography } from '@mui/material';
import axios from 'axios';
import { FormEvent, useEffect, useState } from 'react';
import { Navigate, useLocation } from 'react-router-dom';

import { LogoutMessagePopUp } from '@/components/LogoutMessagePopup';
import { LOGOUT_MESSAGE } from '@/constants/authConstants';
import { LOCAL_LOGIN_ENDPOINT } from '@/constants/queryPaths';
import { DEFAULT_NAVIGATION_ROUTE } from '@/constants/routes';
import { useAuth } from '@/context/AuthContext';

// Контракт localLogin: 200 → text/plain "token=<jwt>",
// ошибка → 401 с текстом ошибки в теле
const TOKEN_FROM_RESPONSE_REGEXP = /token=([^&\s]+)/;

const DEMO_EMAIL = 'modsen.portal.test.ceo@modsen-software.com';
const DEMO_PASSWORD = 'demo1234';

export function SignInPage() {
	const { token, handleReceivedToken } = useAuth();
	const location = useLocation();
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');
	const [error, setError] = useState<string | null>(null);
	const [isSubmitting, setIsSubmitting] = useState(false);
	const [isLogOutPopupOpen, setIsLogOutPopupOpen] = useState(false);
	const errorMessage = localStorage.getItem(LOGOUT_MESSAGE);

	useEffect(() => {
		if (errorMessage) {
			setIsLogOutPopupOpen(true);
		}
	}, [errorMessage]);

	const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
		event.preventDefault();
		if (isSubmitting) {
			return;
		}

		setIsSubmitting(true);
		setError(null);

		try {
			const response = await axios.post<string>(
				LOCAL_LOGIN_ENDPOINT,
				{ email, password }
			);
			const jwt = TOKEN_FROM_RESPONSE_REGEXP.exec(response.data)?.[1];

			if (!jwt) {
				setError('Сервер вернул некорректный ответ авторизации.');

				return;
			}

			handleReceivedToken(jwt);
		} catch (requestError) {
			const apiError =
				axios.isAxiosError(requestError) &&
				typeof requestError.response?.data === 'string' &&
				requestError.response.data
					? requestError.response.data
					: null;

			setError(
				apiError ??
					'Сервер авторизации недоступен. Запустите API и попробуйте снова.'
			);
		} finally {
			setIsSubmitting(false);
		}
	};

	const fillDemoCredentials = () => {
		setEmail(DEMO_EMAIL);
		setPassword(DEMO_PASSWORD);
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
		<Box
			sx={(theme) => ({
				minHeight: '100vh',
				display: 'flex',
				alignItems: 'center',
				justifyContent: 'center',
				padding: 4,
				background: `radial-gradient(
						1200px 640px at 50% -10%,
						rgba(255, 176, 32, 0.08),
						transparent 62%
					), ${theme.palette.background.default}`,
			})}
		>
			<Box
				sx={(theme) => ({
					width: '100%',
					maxWidth: 420,
					padding: 6,
					borderRadius: 3,
					backgroundColor: theme.palette.background.paper,
					border: `1px solid ${theme.palette.table.border}`,
					boxShadow: '0 18px 60px rgba(0, 0, 0, 0.45)',
				})}
			>
				<Typography
					sx={{
						color: 'primary.main',
						textTransform: 'uppercase',
						letterSpacing: '0.35em',
						fontSize: 12,
						fontWeight: 600,
					}}
				>
					Тёмная комната
				</Typography>
				<Typography
					variant="h4"
					sx={{ color: 'text.primary', marginTop: 1, fontWeight: 700 }}
				>
					Фотопринт
				</Typography>
				<Typography sx={{ color: 'text.secondary', marginTop: 1 }}>
					Платформа управления производством печати
				</Typography>

				<Box
					component="form"
					onSubmit={handleSubmit}
					sx={{ display: 'flex', flexDirection: 'column', gap: 2, marginTop: 5 }}
				>
					<TextField
						label="Email"
						type="email"
						value={email}
						onChange={(event) => setEmail(event.target.value)}
						required
						fullWidth
						autoComplete="username"
					/>
					<TextField
						label="Пароль"
						type="password"
						value={password}
						onChange={(event) => setPassword(event.target.value)}
						required
						fullWidth
						autoComplete="current-password"
					/>

					{error && (
						<Typography sx={{ color: 'error.main', fontSize: 14 }}>{error}</Typography>
					)}

					<Button
						type="submit"
						variant="contained"
						disabled={isSubmitting}
						sx={{ marginTop: 2, padding: '12px 0', fontWeight: 600 }}
					>
						{isSubmitting ? 'Входим…' : 'Войти'}
					</Button>

					<Button
						variant="text"
						onClick={fillDemoCredentials}
						sx={{ alignSelf: 'center', fontSize: 13, textTransform: 'none' }}
					>
						Подставить демо-учётку
					</Button>
				</Box>
			</Box>

			{isLogOutPopupOpen && errorMessage && (
				<LogoutMessagePopUp
					handleClose={handleCloseMessagePopup}
					message={errorMessage}
				/>
			)}
		</Box>
	);
}

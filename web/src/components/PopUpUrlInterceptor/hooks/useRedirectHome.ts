import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

import { Routes } from '@/constants/routes';

import { UseRedirectHomeProps } from './types';

export const useRedirectHome = ({
	isRedirect,
	allowedPopUps,
	search,
}: UseRedirectHomeProps) => {
	const navigate = useNavigate();

	useEffect(() => {
		if (isRedirect && allowedPopUps.length) {
			navigate(`${Routes.home}${search}`, { replace: true });
		}
	}, [isRedirect, allowedPopUps.length, search, navigate]);
};

import { PropsWithChildren } from 'react';

import { Routes } from '@/constants/routes';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';

import type { ProtectedViewProps } from './types';

export function ProtectedView({
	roles,
	slug,
	claims,
	children,
}: PropsWithChildren<ProtectedViewProps>) {
	const { role, claims: userClaims } = useAuth();
	// eslint-disable-next-line spaced-comment
	//TODO: delete the added conditional after all claims for the settings and equipment are added

	if (
		slug === Routes.settings ||
		slug === Routes.equipment ||
		slug === Routes.adminPanel
	) {
		return (claims && checkIsHasClaims(claims, userClaims)) ||
			roles.includes(role)
			? children
			: null;
	}

	if (claims && claims.length) {
		return checkIsHasClaims(claims, userClaims) ? children : null;
	}

	if (role) {
		return roles.includes(role) ? children : null;
	}

	return null;
}

import { Navigate, Outlet, useLocation } from 'react-router-dom';

import { Routes } from '@/constants/routes';

import { PrivateRouteProps } from './type';
import { checkIsRedirect } from './utils';

/**
 * Hide routes if user doesn't match the role
 */
export function PrivateRoute({
	available,
	redirectPath = Routes.all,
	availablePermissions,
}: PrivateRouteProps) {
	const { search } = useLocation();
	const isRedirect = checkIsRedirect(available, availablePermissions);

	if (isRedirect) {
		return <Navigate to={`${redirectPath}${search}`} replace />;
	}

	return <Outlet />;
}

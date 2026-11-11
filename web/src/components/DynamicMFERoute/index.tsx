import { useTheme } from '@emotion/react';
import { useParams, useLocation } from 'react-router-dom';

import { MicroFrontend } from '@/components/MicroFrontend';
import { PageLoader } from '@/components/PageLoader';
import { useAuth } from '@/context/AuthContext';
import { NotFoundPage } from '@/pages/NotFound';
import { useGetMicrofrontends } from '@/queries/microfrontends/useGetMicrofrontends';

import { DynamicMFERouteProps } from './types';

export function DynamicMFERoute(props: DynamicMFERouteProps) {
	const { mfeName } = useParams<{ mfeName: string }>();
	const userInfo = useAuth();
	const { data, isLoading, isError } = useGetMicrofrontends();
	const theme = useTheme();
	const location = useLocation();

	if (isLoading) {
		return <PageLoader />;
	}

	if (isError) {
		return <NotFoundPage />;
	}

	const mfeConfig = data?.microfrontends?.find((mfe) => {
		const nameMatch = mfeName && mfe.name === mfeName;
		const routeMatch = mfe.routes?.some((route) => {
			const matches = location.pathname.startsWith(route);

			return matches;
		});

		return nameMatch || routeMatch;
	});

	if (!mfeConfig) {
		return <NotFoundPage />;
	}

	return (
		<MicroFrontend
			{...mfeConfig}
			fallbackComponent={NotFoundPage}
			userInfo={userInfo}
			theme={theme}
			{...props}
		/>
	);
}

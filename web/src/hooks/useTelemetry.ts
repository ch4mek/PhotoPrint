import { useEffect } from 'react';

import { initializeApplicationInsights } from '@/services/telemetryService';

export const useTelemetry = () => {
	useEffect(() => {
		if (
			import.meta.env.MODE !== 'development' &&
			import.meta.env.VITE_VERCEL_ENV !== 'development'
		) {
			initializeApplicationInsights();
		}
	}, []);
};

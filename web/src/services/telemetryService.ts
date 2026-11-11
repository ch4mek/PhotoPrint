/* eslint-disable no-param-reassign */
import { DistributedTracingModes } from '@microsoft/applicationinsights-common';
import { ReactPlugin } from '@microsoft/applicationinsights-react-js';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { createBrowserHistory } from 'history';

export const initializeApplicationInsights = () => {
	const browserHistory = createBrowserHistory({ window });
	const reactPlugin = new ReactPlugin();

	const appInsights = new ApplicationInsights({
		config: {
			connectionString: import.meta.env.VITE_INSIGHTS_CONNECTION_STRING || '',
			enableCorsCorrelation: true,
			distributedTracingMode: DistributedTracingModes.W3C,
			extensions: [reactPlugin],
			extensionConfig: {
				[reactPlugin.identifier]: { history: browserHistory },
			},
			enableAutoRouteTracking: true,
		},
	});

	try {
		appInsights.loadAppInsights();
		appInsights.addTelemetryInitializer((telemetry) => {
			if (!telemetry) {
				return;
			}

			if (telemetry.tags) {
				telemetry.tags['ai.cloud.role'] = 'webui';
			}
		});
	} catch (err) {
		// no need to process: don't use App Insights in development mode,
		// don't use notifications about App Insights init in other modes
	}
};

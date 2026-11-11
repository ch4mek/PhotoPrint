import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import { DragAndDropProvider } from '@/components/DragAndDropProvider';
import { PopUpUrlInterceptor } from '@/components/PopUpUrlInterceptor';
import { ReactErrorBoundary } from '@/components/ReactErrorBoundary';
import { ToastContainer } from '@/components/ToastContainer';
import { AuthProvider } from '@/context/AuthContext';
import { ConfigurationProvider } from '@/context/Configuration';
import { ColorSchemeProvider } from '@/context/Theme';
import { ToastProvider } from '@/context/Toast';
import { AppRouter } from '@/routes/index';

import 'react-data-grid/lib/styles.css';
import 'react-toastify/dist/ReactToastify.css';
import { NonRetryableStatuses } from './constants/httpStatuses';
import { MAX_RETRY_ATTEMPTS } from './constants/queryParams';
import { useDynamicFavicon } from './hooks/useDynamicFavicon';
import { useTelemetry } from './hooks/useTelemetry';

const queryClient = new QueryClient({
	defaultOptions: {
		queries: {
			retry: (failureCount, error: unknown) => {
				const typedError = error as AxiosError;
				const status = typedError?.response?.status;

				const queryIsAborted = typedError?.config?.signal?.aborted;
				const isNonRetryable =
					(status && NonRetryableStatuses.includes(status)) || queryIsAborted;

				const shouldRetry =
					!isNonRetryable && failureCount < MAX_RETRY_ATTEMPTS;

				return shouldRetry;
			},
		},
	},
});

export function App() {
	useTelemetry();
	useDynamicFavicon();

	return (
		<ReactErrorBoundary>
			<LocalizationProvider dateAdapter={AdapterDayjs}>
				<QueryClientProvider client={queryClient}>
					<DragAndDropProvider>
						<ColorSchemeProvider>
							<ToastProvider>
								<AuthProvider>
									<ConfigurationProvider>
										<PopUpUrlInterceptor>
											<AppRouter />
											<ToastContainer position="bottom-right" />
										</PopUpUrlInterceptor>
									</ConfigurationProvider>
								</AuthProvider>
							</ToastProvider>
						</ColorSchemeProvider>
					</DragAndDropProvider>
				</QueryClientProvider>
			</LocalizationProvider>
		</ReactErrorBoundary>
	);
}

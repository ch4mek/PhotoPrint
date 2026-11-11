import { PageLoader } from '@/components/PageLoader';
import { ReactErrorBoundary } from '@/components/ReactErrorBoundary';
import { useMicroFrontend } from '@/hooks/useMicrofrontend';

import { MicroFrontendProps } from './types';

export function MicroFrontend({
	remoteEntries,
	scope,
	modules,
	fallbackComponent: FallbackComponent,
	loading: LoadingComponent = PageLoader,
	errorBoundary: ErrorBoundaryComponent = ReactErrorBoundary,
	...props
}: MicroFrontendProps) {
	const { App, loading, error } = useMicroFrontend(
		remoteEntries,
		scope,
		modules
	);

	if (loading) {
		return <LoadingComponent />;
	}

	if (error || !App) {
		return FallbackComponent ? <FallbackComponent /> : null;
	}

	return (
		<ErrorBoundaryComponent>
			<App {...props} />
		</ErrorBoundaryComponent>
	);
}

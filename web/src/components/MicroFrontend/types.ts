import { ComponentType, PropsWithChildren } from 'react';

import { RemoteEntries } from '@/types/microfrontends';

export interface MicroFrontendProps extends Record<string, unknown> {
	remoteEntries: RemoteEntries | null;
	scope: string;
	modules: string[] | null;
	fallbackComponent?: ComponentType;
	loading?: ComponentType;
	errorBoundary?: ComponentType<PropsWithChildren>;
}

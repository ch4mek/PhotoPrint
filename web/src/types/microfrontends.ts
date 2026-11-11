import { UserClaims } from './claims';

export type RemoteEntries = string;

export interface MicroFrontendConfig {
	name: string;
	version: string;
	remoteEntries: RemoteEntries | string | null;
	routes: string[] | null;
	scope: string;
	modules: string[] | null;
	claims: UserClaims[] | null;
}

export type MicroFrontendExports = Record<string, unknown>;

export interface MFERegistryResponse {
	microfrontends: MicroFrontendConfig[];
}

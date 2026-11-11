import {
	getRemoteContainer,
	initializeContainer,
	loadScript,
} from '@/helpers/microfrontendHelpers';
import { MicroFrontendExports, RemoteEntries } from '@/types/microfrontends';

export async function loadMicroFrontend(
	remoteEntries: RemoteEntries,
	scope: string,
	modules: string[]
) {
	await loadScript(remoteEntries);

	const container = await getRemoteContainer(remoteEntries, scope);

	if (!container) {
		throw new Error(`Container "${scope}" not found`);
	}

	await initializeContainer(container);

	const results = await Promise.all(
		modules.map(async (module) => {
			const factory = await container.get(module);

			return factory() as MicroFrontendExports;
		})
	);

	const [appModule, sharedModule] = results;

	return {
		App: appModule?.default,
		exports: sharedModule ?? {},
	};
}

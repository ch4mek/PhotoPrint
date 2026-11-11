/* eslint-disable no-underscore-dangle */
import { MICROFRONTENDS_SHARED_SCOPE } from '@/constants/microfrontends';
import { RemoteEntries } from '@/types/microfrontends';

export const getRemoteContainer = async (
	remoteEntries: string,
	_scope: string
): Promise<FederatedContainer> => {
	const module = await import(/* @vite-ignore */ remoteEntries);

	if (typeof module.get === 'function' && typeof module.init === 'function') {
		return module as unknown as FederatedContainer;
	}

	const container = module[_scope] ?? module.default;

	if (!container) {
		throw new Error(`Container "${_scope}" not found in module`);
	}

	return container;
};

export const loadScript = (remoteEntries: RemoteEntries): Promise<void> => {
	if (!remoteEntries) {
		return Promise.reject(new Error('Failed no remoteEntries were passed'));
	}

	if (document.querySelector(`script[src="${remoteEntries}"]`)) {
		return Promise.resolve();
	}

	return new Promise((resolve) => {
		const script = document.createElement('script');
		script.src = remoteEntries;
		script.type = 'module';
		script.crossOrigin = '';
		script.onload = () => resolve();
		document.head.appendChild(script);
	});
};

export const initializeContainer = async (container: FederatedContainer) => {
	try {
		await container.init?.(MICROFRONTENDS_SHARED_SCOPE);
	} catch (error) {
		throw new Error(`[MFE] Container init failed:${error}`);
	}
};

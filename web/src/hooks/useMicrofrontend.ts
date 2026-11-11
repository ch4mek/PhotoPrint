import { ComponentType, useEffect, useState } from 'react';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { loadMicroFrontend } from '@/helpers/loadMicroFrontend';
import { NotFoundPage } from '@/pages/NotFound';
import { MicroFrontendExports, RemoteEntries } from '@/types/microfrontends';

export function useMicroFrontend(
	remoteEntries?: RemoteEntries | null,
	scope?: string,
	modules?: string[] | null
) {
	const [app, setApp] = useState<ComponentType | null>(null);
	const [exports, setExports] = useState<MicroFrontendExports>({});
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<Error | null>(null);
	const toast = useToast();

	useEffect(() => {
		if (!remoteEntries || !scope || !modules) {
			return;
		}

		let mounted = true;

		setLoading(true);
		loadMicroFrontend(remoteEntries, scope, modules)
			.then(({ App, exports: mfeExports }) => {
				if (!mounted) {
					return;
				}
				setApp(() => App as ComponentType);
				setExports(mfeExports as MicroFrontendExports);
			})
			.catch((err) => {
				setError(err);
				toast.open(createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG));
			})
			.finally(() => {
				if (mounted) {
					setLoading(false);
				}
			});

		return () => {
			mounted = false;
		};
	}, [remoteEntries, scope, modules]);

	return {
		App: app || NotFoundPage,
		exports,
		loading,
		error,
	};
}

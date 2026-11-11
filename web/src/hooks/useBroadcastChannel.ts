import { useEffect } from 'react';

import { BROADCAST_CHANNEL_NAME } from '@/constants/authConstants';
import { AUTH_STORAGE_KEY } from '@/constants/storageKeys';
import {
	InitialAuthState,
	SessionBroadcastEvents,
	SessionBroadcastMessage,
} from '@/context/types';
import { FORM_BACKUPS_STORAGE_KEY } from '@/forms/components/FormikWithBackup/constants';

const broadcastChannel =
	typeof window.BroadcastChannel === 'function'
		? new window.BroadcastChannel(BROADCAST_CHANNEL_NAME)
		: null;

export const useBroadcastChannel = (
	setIsLoading: React.Dispatch<React.SetStateAction<boolean>>,
	setAuthorization: React.Dispatch<React.SetStateAction<InitialAuthState>>
) => {
	useEffect(() => {
		if (!broadcastChannel) {
			setIsLoading(false);

			return;
		}

		if (!sessionStorage.getItem(AUTH_STORAGE_KEY)) {
			broadcastChannel.postMessage({
				type: SessionBroadcastEvents.requestSession,
			});
			setTimeout(() => {
				setIsLoading(false);
			}, 500);
		} else {
			setIsLoading(false);
		}

		const handleGetSession = (event: MessageEvent) => {
			const message: SessionBroadcastMessage = event.data;

			if (message.type === SessionBroadcastEvents.sendSession) {
				if (sessionStorage.getItem(AUTH_STORAGE_KEY)) {
					return;
				}

				const storageInfo = JSON.parse(message.data as string);

				if (storageInfo.auth) {
					sessionStorage.setItem(AUTH_STORAGE_KEY, storageInfo.auth);
				}

				if (storageInfo.formBackups) {
					sessionStorage.setItem(
						FORM_BACKUPS_STORAGE_KEY,
						storageInfo.formBackups
					);
				}

				setAuthorization(JSON.parse(storageInfo.auth) as InitialAuthState);
				setIsLoading(false);
			}
		};

		const handleSessionSharing = (event: MessageEvent) => {
			if (event.data.type === SessionBroadcastEvents.requestSession) {
				const authValue = sessionStorage.getItem(AUTH_STORAGE_KEY);
				const formBackups = sessionStorage.getItem(FORM_BACKUPS_STORAGE_KEY);

				if (authValue) {
					const message: SessionBroadcastMessage = {
						type: SessionBroadcastEvents.sendSession,
						data: JSON.stringify({ auth: authValue, formBackups }),
					};

					broadcastChannel.postMessage(message);
				}
			}
		};

		broadcastChannel.addEventListener('message', handleSessionSharing);
		broadcastChannel.addEventListener('message', handleGetSession, {
			once: true,
		});

		return () => {
			broadcastChannel.removeEventListener('message', handleGetSession);
			broadcastChannel.removeEventListener('message', handleSessionSharing);
		};
	}, []);
};

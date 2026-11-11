import { useEffect } from 'react';

import { NotificationType, USER_NOT_FOUND } from '@/constants/notifications';
import { PopUpSearchParams } from '@/constants/searchParams';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';

import { UseDeniedPopUpNotificationsProps } from './types';

export const useDeniedPopUpNotifications = ({
	valueDeniedPopUps,
	isUserInitialized,
}: UseDeniedPopUpNotificationsProps) => {
	const toast = useToast();

	useEffect(() => {
		if (!isUserInitialized) {
			return;
		}

		const isOwnProfilePopupDenied = valueDeniedPopUps.includes(
			PopUpSearchParams.UserIdParam
		);

		if (isOwnProfilePopupDenied) {
			toast.open(
				createToast(NotificationType.ERROR, USER_NOT_FOUND, USER_NOT_FOUND)
			);
		}
	}, [valueDeniedPopUps, isUserInitialized, toast]);
};

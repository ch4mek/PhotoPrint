import { useContext } from 'react';

import { NotificationsParameteresContext } from '../NotificationsList/context/NotificationsContext';

import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { PopUpSearchParams } from '@/constants/searchParams';

export const useUserRequestNotification = (
	notificationId: number,
	inquiryId: number,
	read: boolean
) => {
	const { readNotification, handleNotificationsModalClose } = useContext(
		NotificationsParameteresContext
	);
	const handleOpen = useOpenPopUpViaUrl(PopUpSearchParams.InquiryIdParam);

	const handleClick = () => {
		if (!read) {
			readNotification(notificationId);
		}

		handleNotificationsModalClose();
		handleOpen(inquiryId.toString());
	};

	return handleClick;
};

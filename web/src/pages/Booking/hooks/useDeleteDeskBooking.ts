import { useQueryClient } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';

import { NotificationType } from '@/constants/notifications';
import { BookingQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { isMeetingRoom } from '@/helpers/isMeetingRoom';
import { UseDeleteBookingProps } from '@/pages/Booking/types';
import { getBookingDeletingToastMessage } from '@/pages/Booking/utils';
import { useDeleteBooking } from '@/queries/booking';

export const useDeleteDeskBooking = ({ item, id }: UseDeleteBookingProps) => {
	const toast = useToast();
	const queryClient = useQueryClient();
	const { t } = useTranslation('booking');

	const { mutateAsync, isPending } = useDeleteBooking(
		{
			onSuccess: () => {
				queryClient.invalidateQueries({
					queryKey: [BookingQueriesKeys.bookings],
				});
				queryClient.refetchQueries({
					queryKey: [BookingQueriesKeys.reservations],
				});

				if (item && !isMeetingRoom(item)) {
					const { isBlock, userId, isForGuest } = item;

					const isOwnBooking = !isForGuest && userId === id;

					toast.open(
						createToast(
							NotificationType.SUCCESS,
							getBookingDeletingToastMessage(isBlock, isOwnBooking, t)
						)
					);
				}
			},
		},
		toast
	);

	return {
		handleDeskBookingDeleting: mutateAsync,
		isDeskBookingDeletingLoading: isPending,
	};
};

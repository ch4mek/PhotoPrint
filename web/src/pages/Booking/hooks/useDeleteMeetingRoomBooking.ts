import { useQueryClient } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';

import { NotificationType } from '@/constants/notifications';
import { BookingQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { UseDeleteBookingProps } from '@/pages/Booking/types';
import { getBookingDeletingToastMessage } from '@/pages/Booking/utils';
import { useDeleteMeetingRoomBooking as useDeletingMeetingRoomBooking } from '@/queries/booking/useDeleteMeetingRoomBooking';

export const useDeleteMeetingRoomBooking = ({
	id,
	item,
}: UseDeleteBookingProps) => {
	const toast = useToast();
	const queryClient = useQueryClient();
	const { t } = useTranslation('booking');

	const { mutateAsync, isPending } = useDeletingMeetingRoomBooking(
		{
			onSuccess: () => {
				queryClient.refetchQueries({
					queryKey: [BookingQueriesKeys.meetingRoomsBookings],
				});
				queryClient.invalidateQueries({
					queryKey: [BookingQueriesKeys.meetingRoomCollisions],
				});
				queryClient.refetchQueries({
					queryKey: [BookingQueriesKeys.meetingRoomReservations],
				});

				const bookingEntryUserId = item?.userId;
				const isOwnBooking = bookingEntryUserId === id;

				toast.open(
					createToast(
						NotificationType.SUCCESS,
						getBookingDeletingToastMessage(false, isOwnBooking, t)
					)
				);
			},
		},
		toast
	);

	return {
		handleRoomBookingDeleting: mutateAsync,
		isRoomBookingDeletingLoading: isPending,
	};
};

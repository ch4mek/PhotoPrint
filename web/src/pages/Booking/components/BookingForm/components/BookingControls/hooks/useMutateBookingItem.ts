import { useIsFetching, useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';

import { ALL_DAY_TIME, RequestTypes } from '@/constants/booking';
import { HttpStatuses } from '@/constants/httpStatuses';
import {
	NotificationType,
	ALREADY_BOOKED,
	GUEST_ALREADY_BOOKED,
	DESK_IS_ALREADY_BOOKED_RESPONSE_MESSAGE,
} from '@/constants/notifications';
import { BookingQueriesKeys } from '@/constants/queriesKeys';
import { isRegularEmployee } from '@/constants/roles';
import { useBookingContext } from '@/context/Booking';
import { useAuth, useToast } from '@/context/index';
import { SelectedBookingItemType } from '@/context/types';
import {
	getRequestBookingDate,
	getRequestBookingDateTime,
} from '@/helpers/bookingTime';
import { createToast } from '@/helpers/createToast';
import { useCreateBooking } from '@/queries/booking';
import { useCreateGuestBooking } from '@/queries/booking/useCreateGuestBooking';
import { useCreateMeetingRoomBooking } from '@/queries/booking/useCreateMeetingRoomBooking';

export const useMutateBookingItem = (
	isChecked: boolean,
	setIsChecked: React.Dispatch<React.SetStateAction<boolean>>,
	setIsDialogOpen: React.Dispatch<React.SetStateAction<boolean>>,
	setWarning: React.Dispatch<React.SetStateAction<string | null>>
) => {
	const { id, role } = useAuth();
	const { t } = useTranslation('booking', { keyPrefix: 'errors' });

	const { t: tBooking } = useTranslation('booking');

	const toast = useToast();

	const isWorkplaceCollisionFetching = useIsFetching({
		queryKey: [BookingQueriesKeys.collisions],
	});

	const isMeetingRoomCollisionFetching = useIsFetching({
		queryKey: [BookingQueriesKeys.meetingRoomCollisions],
	});

	const {
		selectedItem,
		selectedTime,
		setSelectedItem,
		firstDay,
		lastDay,
		employee,
		office,
		init,
		timelineDate,
		weeksCount,
		isAllDayChecked,
		isSelectedMeetingRoomTimeInPast,
		setGuestName,
		isGuestMode,
		guestName,
	} = useBookingContext();

	const isBookButtonDisabled =
		!selectedItem ||
		isWorkplaceCollisionFetching ||
		isMeetingRoomCollisionFetching;

	const weeksInterval = isChecked ? weeksCount : undefined;

	const timezone = office.timeZone;

	const queryClient = useQueryClient();

	const invalidateBookingQueries = () => {
		queryClient.invalidateQueries({
			queryKey: [BookingQueriesKeys.bookings],
		});
		queryClient.refetchQueries({
			queryKey: [BookingQueriesKeys.reservations],
		});
	};

	const invalidateMettingRoomsQueries = () => {
		queryClient.invalidateQueries({
			queryKey: [BookingQueriesKeys.meetingRoomsBookings],
		});
		queryClient.refetchQueries({
			queryKey: [BookingQueriesKeys.meetingRoomReservations],
		});
	};

	const { mutate } = useCreateBooking({
		onSuccess: () => {
			invalidateBookingQueries();

			setSelectedItem(null);

			toast.open(
				createToast(NotificationType.SUCCESS, tBooking('toasts.deskBooked'))
			);
		},
		onError: (error) => {
			if (error.response?.status === HttpStatuses.ConflictRequest) {
				if (error.response?.data.Message === ALREADY_BOOKED) {
					setWarning(t('alreadyHaveBooking'));
				} else if (
					error.response?.data.Message ===
					DESK_IS_ALREADY_BOOKED_RESPONSE_MESSAGE
				) {
					setWarning(t('selectedDeskBooked'));
					queryClient.invalidateQueries({
						queryKey: [BookingQueriesKeys.bookings],
					});
					queryClient.refetchQueries({
						queryKey: [BookingQueriesKeys.reservations],
					});
					init();
				} else {
					toast.open(createToast(NotificationType.ERROR, t('generic')));
				}
			} else {
				toast.open(createToast(NotificationType.ERROR, t('generic')));
			}
		},
	});

	const { mutate: mutateGuestBooking } = useCreateGuestBooking({
		onSuccess: () => {
			invalidateBookingQueries();

			setSelectedItem(null);
			setGuestName(null);

			toast.open(
				createToast(NotificationType.SUCCESS, tBooking('toasts.deskBooked'))
			);
		},
		onError: (error) => {
			if (error.response?.status === HttpStatuses.ConflictRequest) {
				if (error.response?.data.Message === GUEST_ALREADY_BOOKED) {
					setWarning(t('guestAlreadyBooked'));
				} else {
					setWarning(t('selectedDeskBooked'));
					invalidateBookingQueries();
					init();
				}
			} else {
				toast.open(createToast(NotificationType.ERROR, t('generic')));
			}
			setSelectedItem(null);
			setGuestName(null);
		},
	});

	const { mutate: mutateMeetingRooms } = useCreateMeetingRoomBooking({
		onSuccess: () => {
			invalidateMettingRoomsQueries();
			setSelectedItem(null);
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					tBooking('toasts.meetingRoomBooked')
				)
			);
		},
		onError: () => {
			setWarning(t('meetingRoomAlreadyBooked'));
			invalidateMettingRoomsQueries();
		},
	});

	const handleBookClick = useCallback(() => {
		if (isBookButtonDisabled) {
			return;
		}

		if (
			selectedTime &&
			selectedItem.type === SelectedBookingItemType.Room &&
			isSelectedMeetingRoomTimeInPast
		) {
			toast.open(
				createToast(NotificationType.ERROR, t('cantBookMeetingRoomInPast'))
			);

			return;
		}

		if (
			selectedItem.type === SelectedBookingItemType.Room &&
			firstDay &&
			lastDay &&
			selectedTime
		) {
			mutateMeetingRooms({
				roomId: selectedItem.id,
				reservationFromUtc: getRequestBookingDateTime(
					firstDay,
					selectedTime.from,
					isAllDayChecked,
					ALL_DAY_TIME.from,
					timezone
				),
				reservationToUtc: getRequestBookingDateTime(
					firstDay,
					selectedTime.to,
					isAllDayChecked,
					ALL_DAY_TIME.to,
					timezone
				),
				reservationPeriodFromOfficeDate: getRequestBookingDate(firstDay),
				reservationPeriodToOfficeDate: getRequestBookingDate(lastDay),
				repeatReservationWeeks: weeksInterval || 0,
				userId: employee ? employee.id : id,
			});
		} else if (isGuestMode) {
			mutateGuestBooking({
				workplaceId: selectedItem.id,
				weeksInterval,
				fromDate: firstDay,
				toDate: lastDay,
				requestType: RequestTypes.CreateForGuest,
				officeId: office.id,
				guestName: guestName || '',
			});
		} else {
			mutate({
				workplaceId: selectedItem.id,
				weeksInterval,
				fromDate: firstDay,
				toDate: lastDay,
				requestType: RequestTypes.CreateSelf,
				userId:
					id === employee?.id || isRegularEmployee(role)
						? undefined
						: employee?.id,
				officeId: office.id,
			});
		}

		setIsChecked(false);
		setSelectedItem(null);
	}, [
		selectedTime,
		selectedItem,
		guestName,
		isBookButtonDisabled,
		timelineDate,
		firstDay,
		lastDay,
		weeksInterval,
		isAllDayChecked,
		office,
		employee?.id,
		id,
		isGuestMode,
	]);

	const handleBlock = useCallback(() => {
		if (!selectedItem) {
			return;
		}
		mutate({
			workplaceId: selectedItem.id,
			fromDate: firstDay,
			toDate: lastDay,
			requestType: RequestTypes.CreateBlock,
			userId: employee?.id,
			officeId: office.id,
		});

		setIsDialogOpen(false);
	}, [selectedItem, firstDay, lastDay, employee?.id, id]);

	return {
		handleBookClick,
		handleBlock,
	};
};

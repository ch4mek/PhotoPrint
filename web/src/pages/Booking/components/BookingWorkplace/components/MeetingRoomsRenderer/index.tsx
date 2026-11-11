import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useLayoutEffect, useMemo } from 'react';

import { convertBookingItemToPopupData } from '../../../BookingList/utils';

import { Timeline } from '@/components/Timeline';
import { Section, TimezoneObject } from '@/components/Timeline/types';
import { HIDDEN_MEETING_ROOMS } from '@/constants/booking';
import { BookingQueriesKeys } from '@/constants/queriesKeys';
import { useAuth } from '@/context/AuthContext';
import { useBookingContext } from '@/context/Booking';
import { SelectedRoom } from '@/context/types';
import {
	convertDayjsToDatePickerDate,
	getInitialTime,
	getInitialTimelineDate,
	getOfficeBookingRange,
	getRequestBookingDate,
	isTimelineDateInPast,
} from '@/helpers/bookingTime';
import { filterHiddenItems } from '@/helpers/filterHiddenItems';
import { sortByKey } from '@/helpers/sortByKey';
import dayjs from '@/lib/dayjs';
import { DeleteBookingPopup } from '@/pages/Booking/components/PopUps/DeleteBookingPopup';
import { useDeleteMeetingRoomBooking } from '@/pages/Booking/hooks/useDeleteMeetingRoomBooking';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useGetMeetingRoomReservations } from '@/queries/booking/useGetMeetingRoomsReservation';
import { MeetingRoomBooking } from '@/types/booking';

import { MEETING_ROOM_SORT_KEY } from './constants';
import { MeetingRoomsRendererWrapper } from './styled';
import {
	convertSectionToRoom,
	getRelevantTimeForWorkingHours,
	getSelectedPeriod,
	mapReservationsResponse,
} from './utils';

const TIMELINE_SCROLL_PROPS = {
	scrollToCurrentTime: { behavior: 'smooth', position: 'center' },
	scrollToSelectedPeriod: { behavior: 'auto' },
} as const;

export function MeetingRoomsRenderer() {
	const { id: userId } = useAuth();

	const { removeQueries } = useQueryClient();

	const { handleClose, handleOpen, isOpen } = useShowModal();

	const {
		itemToDelete,
		selectedTime,
		timelineDate,
		isWorkingHours,
		isAllDayChecked,
		selectedItem,
		setItemToDelete,
		setTimelineDate,
		setIsWorkingHours,
		setSelectedTime,
		setSelectedItem,
		setFirstDay,
		setLastDay,
		isTimelineInRange,
		isSelectedRoomAvailable,
		setInitialTimelineDate,
		office,
		isSelectedMeetingRoomTimeInPast,
		isTimelineDateSynchronized,
		areMeetingRoomsCollisionsLoading,
	} = useBookingContext();

	const { handleRoomBookingDeleting, isRoomBookingDeletingLoading } =
		useDeleteMeetingRoomBooking({
			item: itemToDelete,
			id: userId,
		});

	const { timeZone, timeZoneDisplayName, id } = office;

	const convertedItemToDelete = convertBookingItemToPopupData(itemToDelete);

	const handleCloseDeletingPopup = useCallback(() => {
		handleClose();
		setSelectedItem(null);
		setItemToDelete(null);
	}, []);

	const handleBookingDeleting = useCallback(async () => {
		if (!itemToDelete) {
			return;
		}

		const meetingRoomItem = itemToDelete as MeetingRoomBooking;

		await handleRoomBookingDeleting({ entryId: meetingRoomItem.id });
		handleCloseDeletingPopup();
	}, [itemToDelete]);

	const initiateBookDelete = useCallback(
		(booking: MeetingRoomBooking | null) => {
			if (booking) {
				setItemToDelete(booking);
			}
		},
		[setItemToDelete]
	);

	const handleWorkingHoursChange = (areHoursWorking: boolean) => {
		setIsWorkingHours(areHoursWorking);

		if (areHoursWorking) {
			const relevantTime = getRelevantTimeForWorkingHours(
				selectedTime,
				areHoursWorking
			);

			if (relevantTime) {
				setSelectedTime(relevantTime);
			}
		}
	};

	useLayoutEffect(() => {
		if (!isTimelineDateSynchronized) {
			setInitialTimelineDate(getInitialTimelineDate(timeZone));
		}

		if (!selectedTime) {
			setSelectedTime(getInitialTime(isWorkingHours, timeZone));
		}
	}, [timeZone]);

	const { data, isLoading } = useGetMeetingRoomReservations(
		getRequestBookingDate(timelineDate),
		id
	);

	const parsedReservations = useMemo(() => {
		if (!data) {
			return [];
		}

		const filteredReservations = filterHiddenItems(
			mapReservationsResponse(data),
			HIDDEN_MEETING_ROOMS,
			MEETING_ROOM_SORT_KEY
		);

		return sortByKey(filteredReservations, MEETING_ROOM_SORT_KEY);
	}, [data]);

	const handleTimePeriodSelection = (
		section: Section,
		from: Date,
		to: Date
	) => {
		setSelectedItem(convertSectionToRoom(section));

		if (!isAllDayChecked) {
			setSelectedTime({
				from: dayjs(from),
				to: dayjs(to),
			});
		}

		setFirstDay(timelineDate.clone());
		setLastDay(timelineDate.clone());
	};

	const selectedTimePeriod = getSelectedPeriod(
		selectedItem,
		selectedTime,
		isTimelineInRange,
		!isSelectedRoomAvailable || isSelectedMeetingRoomTimeInPast,
		isAllDayChecked,
		areMeetingRoomsCollisionsLoading
	);

	const handleForwardClick = () => {
		setTimelineDate(timelineDate.clone().add(1, 'day'));
		removeQueries({
			queryKey: [BookingQueriesKeys.meetingRoomReservations, timelineDate],
		});
	};

	const handleBackClick = () => {
		setTimelineDate(timelineDate.clone().subtract(1, 'day'));
		removeQueries({
			queryKey: [BookingQueriesKeys.meetingRoomReservations, timelineDate],
		});
	};

	const handleTodayClick = () => {
		const currentOfficeDate = dayjs().tz(timeZone).startOf('day');
		const timelineDateWithTimeZone = timelineDate.tz(timeZone);

		if (!timelineDateWithTimeZone.isSame(currentOfficeDate, 'day')) {
			setTimelineDate(currentOfficeDate);
			removeQueries({
				queryKey: [BookingQueriesKeys.meetingRoomReservations, timelineDate],
			});
		}
	};

	const { currentOfficeDateInThreeMonth } = useMemo(
		() => getOfficeBookingRange(timeZone),
		[timeZone]
	);

	const isDisabled =
		isTimelineDateInPast(timelineDate, timeZone) ||
		timelineDate.isAfter(currentOfficeDateInThreeMonth, 'date');

	const handleSectionSelection = (section: Section) => {
		if (selectedItem && selectedItem.id === section.meetingRoomId) {
			setSelectedItem(null);

			return;
		}

		setSelectedItem(convertSectionToRoom(section));
	};

	const timeZoneObject: TimezoneObject = useMemo(() => {
		const offset = dayjs().tz(timeZone).format('Z');

		return {
			timezone: timeZone,
			timezoneOffset: offset,
			timezoneDisplayName: timeZoneDisplayName,
		};
	}, [timeZone, timeZoneDisplayName]);

	const selectedDate = convertDayjsToDatePickerDate(timelineDate) || new Date();

	return (
		<>
			<MeetingRoomsRendererWrapper>
				<Timeline
					{...TIMELINE_SCROLL_PROPS}
					data={parsedReservations}
					selectedDate={selectedDate}
					showLoader={isLoading}
					selectedTimePeriod={selectedTimePeriod}
					isWorkingHoursOnly={isWorkingHours}
					onIsWorkingHoursOnlyChange={handleWorkingHoursChange}
					onTimePeriodSelection={handleTimePeriodSelection}
					onGoForwardDate={handleForwardClick}
					onGoBackwardDate={handleBackClick}
					onGoTodayDate={handleTodayClick}
					selectFromPeriodStart
					isPeriodSelectionDisabled={isDisabled}
					selectedSectionId={selectedItem?.id}
					selectedSectionName={(selectedItem as SelectedRoom)?.name}
					onSectionSelection={handleSectionSelection}
					timezone={timeZoneObject}
					meetingRoomsConfig={office.meetingRooms}
					onSetItemToDelete={initiateBookDelete}
					handleCancelClick={handleOpen}
				/>
			</MeetingRoomsRendererWrapper>

			{isOpen && convertedItemToDelete && (
				<DeleteBookingPopup
					isLoading={isRoomBookingDeletingLoading}
					item={convertedItemToDelete}
					handleSubmit={handleBookingDeleting}
					handleClose={handleCloseDeletingPopup}
				/>
			)}
		</>
	);
}

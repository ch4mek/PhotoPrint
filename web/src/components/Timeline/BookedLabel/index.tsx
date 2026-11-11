import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { ClockIcon } from '@mms/mms-ui-library/icons/base/ClockIcon';
import { LoopIcon } from '@mms/mms-ui-library/icons/base/LoopIcon';
import { memo, MouseEvent, useCallback, useMemo, useRef } from 'react';
import { useTranslation } from 'react-i18next';

import { TimelinePeriod } from '../constants';
import { useAnyElementIsCut } from '../hooks/useAnyElementIsCut';
import { useTimelineContext } from '../TimelineContext';
import { convertTimeToCellIndex } from '../utils/convertTimeToCellIndex';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { useBookingContext } from '@/context/Booking';
import { createPeriodInText } from '@/helpers/booking';
import {
	getFormattedTime,
	getFormattedTimePeriod,
} from '@/helpers/bookingFormatters';
import { isMeetingRoomCanceliationDisabled } from '@/helpers/bookingTime';
import { getFullName } from '@/helpers/formatName';
import { useOpenPersonalInfo } from '@/hooks/useOpenPersonalInfo';
import dayjs from '@/lib/dayjs';
import { getRoomBookingEdgeDates } from '@/pages/Booking/components/BookingList/utils';
import { findMeetingRoomByEntryId } from '@/pages/Booking/components/BookingWorkplace/components/MeetingRoomsRenderer/utils';
import { CancelBookingPopover } from '@/pages/Booking/components/PopUps/CancelBookingPopover';
import { CancelType } from '@/pages/Booking/types';
import { getMeetingRoomDateList } from '@/pages/Booking/utils';
import { useShowModal } from '@/pages/hooks/useShowModal';

import {
	BookedLabelContainer,
	BookedLabelWrapper,
	BookedTimePeriod,
	BookedTimePeriodRow,
	ClockIconWrapper,
	FixedTypography,
	LoopIconWrapper,
} from './styles';
import { TooltipContent } from './TooltipContent';
import { BookedLabelProps } from './types';
import { getVisibleBookingRange, getXPos, getWidth } from './utils';

const { Booking } = CancelType;

function BookedLabelInner({
	id,
	userId,
	fromDateTime,
	toDateTime,
	userFirstName,
	userLastName,
	isRepetitive,
	selectedSectionName,
	onSetItemToDelete,
	handleCancelClick,
	isSelected = false,
}: BookedLabelProps) {
	const { palette } = useTheme();

	const fullNameTextRef = useRef<HTMLSpanElement>(null);
	const timeTextRef = useRef<HTMLSpanElement>(null);
	const meetingRoomRef = useRef<HTMLDivElement>(null);

	const { handleOpen, handleClose, isOpen } = useShowModal();
	const { t } = useTranslation('common');
	const { t: tBooking } = useTranslation('booking');

	const { handleClickName } = useOpenPersonalInfo({
		userId,
		onClose: handleClose,
	});

	const userInitials = getFullName(userFirstName, userLastName);

	const {
		isWorkingHoursOnly,
		isPeriodSelectionDisabled,
		baseHour,
		cellsAmount,
		timezone: { timezone },
	} = useTimelineContext();

	const {
		timelineDate,
		bookingsState: { rooms },
	} = useBookingContext();

	const { isCut } = useAnyElementIsCut([fullNameTextRef, timeTextRef]);

	const startCellIndex = useMemo(
		() => convertTimeToCellIndex(fromDateTime, baseHour),
		[fromDateTime, baseHour]
	);

	const {
		startCellIndex: visibleStartCellIndex,
		duration,
		isClippedOnRight,
	} = useMemo(
		() =>
			getVisibleBookingRange(
				fromDateTime,
				toDateTime,
				baseHour,
				cellsAmount,
				isWorkingHoursOnly
			),
		[fromDateTime, toDateTime, baseHour, cellsAmount, isWorkingHoursOnly]
	);

	const IS_FULLY_OUT_OF_RANGE = duration <= 0;

	const IS_FULL_DAY = duration === TimelinePeriod.FULL_DAY;
	const IS_QUARTER_PERIOD = duration <= TimelinePeriod.QUARTER_PERIOD;
	const IS_LESS_THAN_HOUR_PERIOD = duration < TimelinePeriod.HOUR_PERIOD;
	const IS_PERIOD_WITH_DETAILS =
		duration < TimelinePeriod.HALF_AND_QUARTER_PERIOD;

	const IS_TOOLTIP_OPEN = startCellIndex < 0 || isCut || IS_PERIOD_WITH_DETAILS;

	const timePeriod = useMemo(() => {
		if (IS_FULL_DAY) {
			return t('allDay');
		}

		const fromTime = getFormattedTime(
			fromDateTime.hours.toString(),
			fromDateTime.minutes.toString(),
			true
		);

		const toTime = getFormattedTime(
			toDateTime.hours.toString(),
			toDateTime.minutes.toString(),
			true
		);

		return getFormattedTimePeriod(fromTime, toTime, true, t);
	}, [
		IS_FULL_DAY,
		fromDateTime.hours,
		fromDateTime.minutes,
		toDateTime.hours,
		toDateTime.minutes,
	]);

	const IS_BOOKED_TIME_PERIOD_DISPLAY = !isRepetitive || !IS_QUARTER_PERIOD;

	const booking = useMemo(
		() => findMeetingRoomByEntryId(rooms, id),
		[rooms, id]
	);

	const handleClick = useCallback(() => {
		if (!booking) {
			return;
		}

		if (meetingRoomRef.current) {
			meetingRoomRef.current.scrollIntoView({
				behavior: 'smooth',
				block: 'nearest',
				inline: 'nearest',
			});
		}

		handleOpen();
		onSetItemToDelete?.(booking);
	}, [handleOpen, onSetItemToDelete, booking]);

	const handleSubmit = useCallback(() => {
		if (handleCancelClick) {
			handleClose();
			handleCancelClick();
			onSetItemToDelete?.(null);
		}
	}, [handleCancelClick, handleClose, onSetItemToDelete]);

	const handlePopupClose = useCallback(() => {
		handleClose();
		onSetItemToDelete?.(null);
	}, [handleClose, onSetItemToDelete]);

	const handleWrapperClick = useCallback((e: MouseEvent<HTMLDivElement>) => {
		e.stopPropagation();
	}, []);

	const isHasAccess = useMemo(() => {
		if (!booking) {
			return false;
		}

		const bookedDates = getMeetingRoomDateList(
			booking.reservationPeriodFromOfficeDate,
			booking.reservationPeriodToOfficeDate,
			booking.isRepetitiveBooking
		);

		const currentDate =
			booking.reservations.length > 1 && timelineDate
				? timelineDate.format(YYYY_MM_DD_DAYJS_DATE_FORMAT)
				: bookedDates[0];

		const bookingStart = dayjs(currentDate)
			.hour(fromDateTime.hours)
			.minute(fromDateTime.minutes)
			.second(0)
			.millisecond(0)
			.tz(timezone);

		const bookingEnd = dayjs(currentDate)
			.hour(toDateTime.hours)
			.minute(toDateTime.minutes)
			.second(0)
			.millisecond(0)
			.tz(timezone);

		const isPastBooking = isMeetingRoomCanceliationDisabled(
			bookingStart,
			bookingEnd,
			timezone
		);

		return !isPastBooking;
	}, [booking, fromDateTime, toDateTime, timezone]);

	const bookedDate = useMemo(() => {
		if (booking) {
			const { firstDate, lastDate } = getRoomBookingEdgeDates(
				booking.reservations
			);
			const info = getMeetingRoomDateList(
				firstDate,
				lastDate,
				booking.isRepetitiveBooking
			);

			return createPeriodInText(booking.weeksInInterval || 0, info, t);
		}

		if (fromDateTime && toDateTime && timelineDate) {
			const startDate = timelineDate
				.hour(fromDateTime.hours)
				.minute(fromDateTime.minutes)
				.toISOString();

			const dateList: Array<string> = [startDate];

			return createPeriodInText(0, dateList, t);
		}

		return null;
	}, [booking, fromDateTime, toDateTime, timelineDate]);

	if (isWorkingHoursOnly && IS_FULLY_OUT_OF_RANGE) {
		return null;
	}

	return (
		<>
			<BookedLabelContainer
				width={getWidth(duration)}
				posX={getXPos(visibleStartCellIndex)}
				ref={meetingRoomRef}
			>
				<Tooltip
					open={IS_TOOLTIP_OPEN}
					text={
						<TooltipContent fullName={userInitials} timePeriod={timePeriod} />
					}
				>
					<BookedLabelWrapper
						isDisabled={isPeriodSelectionDisabled}
						hideRightPadding={IS_LESS_THAN_HOUR_PERIOD}
						isClippedOnRight={isClippedOnRight}
						onClick={handleClick}
					>
						<FixedTypography
							ref={fullNameTextRef}
							color={palette['text-black']}
							variant="s-600"
						>
							{userInitials}
						</FixedTypography>
						<BookedTimePeriod>
							{IS_BOOKED_TIME_PERIOD_DISPLAY && (
								<BookedTimePeriodRow>
									<ClockIconWrapper>
										<ClockIcon />
									</ClockIconWrapper>
									<FixedTypography
										ref={timeTextRef}
										color={palette['text-black']}
										variant="s-400"
									>
										{timePeriod}
									</FixedTypography>
								</BookedTimePeriodRow>
							)}
							{isRepetitive && (
								<LoopIconWrapper isQuarterPeriod={IS_QUARTER_PERIOD}>
									<LoopIcon />
								</LoopIconWrapper>
							)}
						</BookedTimePeriod>
					</BookedLabelWrapper>
				</Tooltip>
			</BookedLabelContainer>

			{isOpen && !!isSelected && isHasAccess && (
				<CancelBookingPopover
					{...{
						bookedDate,
						bookedDateTime: timePeriod,
						spaceInfo: `${tBooking(
							'labels.meetingRoom'
						)} ${selectedSectionName}`,
						handleSubmit,
					}}
					handleClose={handlePopupClose}
					distanceToAnchorElement={8}
					type={Booking}
					ref={meetingRoomRef}
					isOpen={isOpen}
					bookedUserInfo={userInitials}
					onWrapperClick={handleWrapperClick}
					handleInfoClick={handleClickName}
				/>
			)}
		</>
	);
}

export const BookedLabel = memo(BookedLabelInner);

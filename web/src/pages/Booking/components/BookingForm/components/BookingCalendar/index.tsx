import { PickersDayProps } from '@mui/x-date-pickers';
import { DayCalendarSlotsComponentsProps } from '@mui/x-date-pickers/internals';
import { useQueryClient } from '@tanstack/react-query';
import {
	ComponentType,
	SyntheticEvent,
	useCallback,
	useMemo,
	useState,
} from 'react';

import { currentDate } from '@/constants/date';
import { BookingQueriesKeys } from '@/constants/queriesKeys';
import { useBookingContext } from '@/context/index';
import { CalendarHighlightedDays } from '@/context/types';
import {
	getEndOfMonthForRequest,
	getStartOfMonthForRequest,
} from '@/helpers/booking';
import { getOfficeBookingRange } from '@/helpers/bookingTime';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { Timestamp } from '@/types/booking';

import { Day } from './Day';
import { CustomCalendarUI } from './styled';
import { getNewHighlightedDays } from './utils';

export function BookingCalendar() {
	const {
		firstDay,
		setFirstDay,
		lastDay,
		setLastDay,
		highlightedDays,
		office,
	} = useBookingContext();

	const [endOfMonth, setEndOfMonth] = useState(
		getEndOfMonthForRequest(lastDay)
	);

	const { currentOfficeDateInThreeMonth, currentOfficeDate } = useMemo(
		() => getOfficeBookingRange(office.timeZone),
		[office.timeZone]
	);

	const [startOfMonth, setStartOfMonth] = useState(
		getStartOfMonthForRequest(lastDay)
	);

	const [hovered, setHovered] = useState(false);
	const [hoveredDateRange, setHoveredDateRange] = useState<(Dayjs | null)[]>(
		[]
	);

	const newHighlightedDays: Record<keyof CalendarHighlightedDays, number[]> =
		useMemo(
			() =>
				getNewHighlightedDays(
					highlightedDays,
					lastDay,
					startOfMonth,
					endOfMonth
				),
			[highlightedDays, startOfMonth, endOfMonth, lastDay]
		);

	const isSameDate =
		firstDay && lastDay ? firstDay.isSame(lastDay, 'day') : false;

	const queryClient = useQueryClient();

	const handleMonthChange = useCallback((days: Dayjs) => {
		setEndOfMonth(getEndOfMonthForRequest(days));
		setStartOfMonth(getStartOfMonthForRequest(days));
		queryClient.cancelQueries({ queryKey: [BookingQueriesKeys.reservations] });
	}, []);

	const getAvailableDateRange = useCallback(
		(day: Dayjs | null) => {
			if (day?.isBefore(currentDate, 'day')) {
				return [currentDate, firstDay];
			}

			if (day?.isAfter(currentOfficeDateInThreeMonth, 'day')) {
				return [firstDay, currentOfficeDateInThreeMonth];
			}

			const dateRange = [day, firstDay];

			return day?.isBefore(firstDay) ? dateRange : dateRange.reverse();
		},
		[firstDay]
	);

	const handleMouseEnterDay = useCallback(
		(event: SyntheticEvent, day: unknown) => {
			if (!hovered) {
				return;
			}

			if ((day as Dayjs).isSame(firstDay)) {
				setHoveredDateRange([]);

				return;
			}

			setHoveredDateRange(getAvailableDateRange(day as Dayjs));
		},
		[hovered, firstDay]
	);

	const handleMouseLeaveDay = useCallback(() => {
		setHoveredDateRange([]);
	}, []);

	// If you want to change management to date from inputs to calendar, use handleChange instead handleChangeFirstDay, handleChangeLastDay and disable inputs.
	const handleChange = useCallback(
		(newValue: Timestamp | null) => {
			const dayjsNewValue = dayjs(newValue);

			if (!lastDay && firstDay && dayjsNewValue?.isBefore(firstDay, 'day')) {
				setFirstDay(dayjsNewValue);
				setLastDay(firstDay);
				setHovered(false);
				setHoveredDateRange([]);

				return;
			}

			if (!firstDay || lastDay) {
				setFirstDay(dayjsNewValue);
				setHovered(true);
				setLastDay(null);
			} else {
				setLastDay(dayjsNewValue);
				setHovered(false);
				setHoveredDateRange([]);
			}
			setEndOfMonth(getEndOfMonthForRequest(dayjsNewValue));
			setStartOfMonth(getStartOfMonthForRequest(dayjsNewValue));
		},
		[firstDay, lastDay, setFirstDay, setLastDay]
	);

	return (
		<CustomCalendarUI
			value={lastDay?.valueOf()}
			onChange={handleChange}
			isInteractive={false}
			onMonthChange={handleMonthChange}
			slots={{ day: Day as ComponentType<PickersDayProps<Dayjs>> }}
			slotProps={{
				day: {
					selectedDay: currentDate.clone(),
					highlightedDays: {
						...newHighlightedDays,
					},
					firstDay,
					lastDay,
					isSameDate,
					hovered,
					hoveredDateRange,
					lastAvailableDay: currentOfficeDateInThreeMonth,
					onMouseEnter: handleMouseEnterDay,
					onMouseLeave: handleMouseLeaveDay,
				} as DayCalendarSlotsComponentsProps<Dayjs>,
			}}
			minDate={currentOfficeDate.valueOf()}
			maxDate={currentOfficeDateInThreeMonth.valueOf()}
			disabled
		/>
	);
}

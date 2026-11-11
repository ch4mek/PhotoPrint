import { Badge } from '@mui/material';
import { PickersDay } from '@mui/x-date-pickers/PickersDay';
import { FunctionComponent, memo } from 'react';

import dayjs from '@/lib/dayjs';

import { NUMBER_OF_MONDAY, NUMBER_OF_SUNDAY } from './constants';
import { CustomPickersDay } from './styled';
import type { DayInnerProps, OtherPropsType } from './types';

function DayInner({
	day,
	selectedDay,
	highlightedDays,
	outsideCurrentMonth,
	firstDay,
	lastDay,
	isSameDate,
	hovered,
	hoveredDateRange,
	lastAvailableDay,
	onMouseEnter,
	onMouseLeave,
	...other
}: DayInnerProps) {
	const isSelected =
		!outsideCurrentMonth && highlightedDays.yourList.indexOf(day.date()) >= 0;
	const isBlocked =
		!outsideCurrentMonth &&
		highlightedDays.blockedList.indexOf(day.date()) >= 0;

	if (selectedDay === null) {
		return <PickersDay day={day} {...(other as OtherPropsType)} />;
	}

	const dayIsBetween = dayjs(day).isBetween(firstDay, lastDay, undefined, '[]');
	const daysIsHoveredBetween =
		hoveredDateRange.length &&
		dayjs(day).isBetween(hoveredDateRange[0], hoveredDateRange[1], 'day', '[]');

	const isFirstDay = dayjs(day).isSame(firstDay, 'day');
	const isFirstHoveredDay =
		hoveredDateRange[0] && dayjs(day).isSame(hoveredDateRange[0], 'day');
	const isLastDay = dayjs(day).isSame(lastDay, 'day');
	const isLastHoveredDay =
		hoveredDateRange[1] && dayjs(day).isSame(hoveredDateRange[1], 'day');
	const isDisabled = dayjs(day).isSameOrAfter(lastAvailableDay);
	const isDayEqualsLastDay = dayjs(day).isSame(lastDay, 'day');
	const isDayEqualsFirstDay = dayjs(day).isSame(firstDay, 'day');

	const isRange =
		isDayEqualsFirstDay !== isDayEqualsLastDay &&
		(isDayEqualsFirstDay || isDayEqualsLastDay);

	const isFirstDayOfWeek = day.isoWeekday() === NUMBER_OF_MONDAY;
	const isLastDayOfWeek = day.isoWeekday() === NUMBER_OF_SUNDAY;

	return (
		<Badge
			key={day.toString()}
			overlap="circular"
			variant="dot"
			invisible={!isBlocked && !isSelected}
			onMouseEnter={onMouseEnter(day)}
			onMouseLeave={onMouseLeave}
		>
			<CustomPickersDay
				{...(other as OtherPropsType)}
				day={day}
				id={isRange ? 'rangeDays' : ''}
				dayIsBetween={dayIsBetween}
				isFirstDay={isFirstDay}
				isFirstHoveredDay={isFirstHoveredDay}
				isLastHoveredDay={isLastHoveredDay}
				isLastDay={isLastDay}
				isDayEqualsLastDay={isDayEqualsLastDay}
				isDayEqualsFirstDay={isDayEqualsFirstDay}
				completeRange={firstDay && lastDay}
				className={isDisabled ? `Mui-disabled` : ''}
				hovered={hovered}
				isSameDate={isSameDate}
				outsideCurrentMonth={outsideCurrentMonth}
				daysIsHoveredBetween={daysIsHoveredBetween}
				isFirstDayOfWeek={isFirstDayOfWeek}
				isLastDayOfWeek={isLastDayOfWeek}
			/>
		</Badge>
	);
}

export const Day = memo(DayInner) as FunctionComponent<DayInnerProps>;

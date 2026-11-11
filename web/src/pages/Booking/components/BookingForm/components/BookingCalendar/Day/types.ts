import { Theme } from '@emotion/react';
import { PickersDayProps } from '@mui/x-date-pickers/PickersDay';
import { MouseEventHandler } from 'react';

import { CalendarHighlightedDays } from '@/context/types';
import { Dayjs } from '@/lib/dayjs';

export interface CustomPickersDayProps extends PickersDayProps<Dayjs> {
	theme?: Theme;
	dayIsBetween: boolean;
	isFirstDay: boolean;
	isLastDay: boolean;
	isSameDate: boolean;
	completeRange: Dayjs | null;
	hovered: boolean;
	daysIsHoveredBetween: boolean | 0;
	isFirstHoveredDay: boolean;
	isLastHoveredDay: boolean;
	isDayEqualsLastDay: boolean;
	isDayEqualsFirstDay: boolean;
	isFirstDayOfWeek: boolean;
	isLastDayOfWeek: boolean;
}

export interface DayInnerProps extends PickersDayProps<Dayjs> {
	day: Dayjs;
	selectedDay?: Dayjs | null;
	highlightedDays: CalendarHighlightedDays;
	outsideCurrentMonth: boolean;
	firstDay: Dayjs | null;
	lastDay: Dayjs | null;
	lastAvailableDay: Dayjs | null;
	isSameDate: boolean;
	hovered: boolean;
	hoveredDateRange: Dayjs[];
	onMouseEnter: (day: unknown) => MouseEventHandler<HTMLSpanElement>;
	onMouseLeave: () => void;
}

export interface OtherPropsType {
	onDaySelect: () => void;
	outsideCurrentMonth: boolean;
	isFirstVisibleCell: boolean;
	isLastVisibleCell: boolean;
}

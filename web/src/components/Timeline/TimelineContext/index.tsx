import { createContext, useContext, useMemo, useState } from 'react';

import {
	BUSSINESS_HOURS_START,
	ALL_HOURS_START,
	BUSINESS_HOURS_CELLS_AMOUNT,
	ALL_HOURS_CELLS_AMOUNT,
} from '../constants';

import { TIMELINE_CONTEXT_INITIAL_STATE } from './constants';
import { TimelineContextProps, TimelineProviderProps } from './types';

const TimelineContext = createContext<TimelineContextProps>(
	TIMELINE_CONTEXT_INITIAL_STATE
);

export const useTimelineContext = () => useContext(TimelineContext);

export function TimelineProvider({
	isWorkingHoursOnly,
	selectFromPeriodStart,
	selectionPeriod,
	children,
	isPeriodSelectionDisabled,
	hasCurrentTimeIndicator,
	showLoader,
	scrollToCurrentTime,
	scrollToSelectedPeriod,
	timezone,
	isSelectedDayToday,
}: TimelineProviderProps) {
	const baseHour = isWorkingHoursOnly ? BUSSINESS_HOURS_START : ALL_HOURS_START;

	const [timelineSectionsHeight, setTimelineSectionsHeight] = useState({});

	const cellsAmount = isWorkingHoursOnly
		? BUSINESS_HOURS_CELLS_AMOUNT
		: ALL_HOURS_CELLS_AMOUNT;

	const contextValue = useMemo(
		() => ({
			isWorkingHoursOnly,
			selectFromPeriodStart,
			selectionPeriod,
			isPeriodSelectionDisabled,
			hasCurrentTimeIndicator,
			showLoader,
			baseHour,
			cellsAmount,
			scrollToCurrentTime,
			scrollToSelectedPeriod,
			timezone,
			isSelectedDayToday,
			timelineSectionsHeight,
			setTimelineSectionsHeight,
		}),
		[
			isWorkingHoursOnly,
			selectionPeriod,
			selectFromPeriodStart,
			isPeriodSelectionDisabled,
			hasCurrentTimeIndicator,
			showLoader,
			baseHour,
			cellsAmount,
			scrollToCurrentTime,
			scrollToSelectedPeriod,
			timezone,
			isSelectedDayToday,
			timelineSectionsHeight,
		]
	);

	return (
		<TimelineContext.Provider value={contextValue}>
			{children}
		</TimelineContext.Provider>
	);
}

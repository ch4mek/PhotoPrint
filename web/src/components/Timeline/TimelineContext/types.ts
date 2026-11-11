import { Dispatch, ReactNode, SetStateAction } from 'react';

import { TimelineProps, TimelineScroll } from '../types';

interface TimelineHeight {
	[key: number]: number;
}
export interface TimelineContextProps
	extends Required<
		Pick<
			TimelineProps,
			| 'isWorkingHoursOnly'
			| 'selectFromPeriodStart'
			| 'selectionPeriod'
			| 'isPeriodSelectionDisabled'
			| 'hasCurrentTimeIndicator'
			| 'showLoader'
			| 'timezone'
		>
	> {
	baseHour: number;
	cellsAmount: number;
	scrollToCurrentTime?: TimelineScroll;
	scrollToSelectedPeriod?: TimelineScroll;
	isSelectedDayToday: boolean;
	timelineSectionsHeight: TimelineHeight;
	setTimelineSectionsHeight: Dispatch<SetStateAction<TimelineHeight>>;
}

export interface TimelineProviderProps
	extends Omit<
		TimelineContextProps,
		| 'baseHour'
		| 'cellsAmount'
		| 'setTimelineSectionsHeight'
		| 'timelineSectionsHeight'
	> {
	children: ReactNode;
}

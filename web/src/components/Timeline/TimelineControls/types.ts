import { ChangeEvent } from 'react';

import { TimelineProps, TimezoneObject } from '../types';

export interface TimelineControlsProps
	extends Pick<
		TimelineProps,
		| 'isWorkingHoursOnly'
		| 'selectedDate'
		| 'onGoBackwardDate'
		| 'onGoForwardDate'
		| 'onGoTodayDate'
	> {
	onWorkingHoursChange: (
		event: ChangeEvent<HTMLInputElement>,
		{ checked }: { checked: boolean }
	) => void;
	timezone: TimezoneObject;
	isSelectedDayToday: boolean;
}

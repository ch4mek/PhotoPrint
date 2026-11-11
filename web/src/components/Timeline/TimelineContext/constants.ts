import {
	BUSINESS_HOURS_CELLS_AMOUNT,
	BUSSINESS_HOURS_START,
	DEFAULT_TIMEZONE,
} from '../constants';

import { TimelineContextProps } from './types';

export const TIMELINE_CONTEXT_INITIAL_STATE: TimelineContextProps = {
	selectFromPeriodStart: true,
	isPeriodSelectionDisabled: false,
	selectionPeriod: 1,
	hasCurrentTimeIndicator: true,
	showLoader: false,
	isWorkingHoursOnly: true,
	cellsAmount: BUSINESS_HOURS_CELLS_AMOUNT,
	baseHour: BUSSINESS_HOURS_START,
	timezone: DEFAULT_TIMEZONE,
	isSelectedDayToday: false,
	timelineSectionsHeight: {},
	setTimelineSectionsHeight: () => {},
};

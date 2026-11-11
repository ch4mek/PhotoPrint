import { ByPageResponse } from '../types';

import { Dayjs } from '@/lib/dayjs';

export enum CalendarExclusions {
	Holiday = 'Holiday',
	DayOff = 'DayOff',
}

export interface GetCalendarExclusionsItem {
	id: number | null;
	name: string | null;
	date: string;
	newDate: string | null;
	dateType: CalendarExclusions;
	countryId: number;
}

export interface CalendarExclusionsParams {
	id?: number | null;
	name?: string | null;
	newDate?: string | null | Dayjs;
	date: string | null | Dayjs;
	dateType: CalendarExclusions;
	countryId: number;
}

export interface GetCalendarExclusionsLookupItem {
	id: number;
	name: string;
}

export type CalendarExclusionsResponse =
	ByPageResponse<GetCalendarExclusionsItem>;

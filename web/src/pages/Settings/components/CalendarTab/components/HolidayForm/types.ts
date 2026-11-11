import { Dayjs } from '@/lib/dayjs';
import { CalendarExclusions } from '@/queries/calendar/types';

export type HolidayFormValues = {
	countryId: string | null;
	dateType: CalendarExclusions;
	date: null | Dayjs;
	newDate: null | Dayjs;
	name: string | null;
};

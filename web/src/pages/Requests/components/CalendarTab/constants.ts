import { CalendarDate } from './types';

const CURRENT_DATE = new Date();

export const INITIAL_SELECTED_DATE: CalendarDate = {
	month: CURRENT_DATE.getMonth(),
	year: CURRENT_DATE.getFullYear(),
};

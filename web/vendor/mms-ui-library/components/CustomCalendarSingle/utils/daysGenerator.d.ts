import { Dayjs } from 'dayjs';
import { BaseCalendarDay, CalendarConfigInput } from '../types';
export declare function getMonthDates(date: Dayjs, config: CalendarConfigInput, selectedDate?: Dayjs | null): BaseCalendarDay[];

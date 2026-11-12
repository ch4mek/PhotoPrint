import { Dayjs } from 'dayjs';
import { CalendarWeekStartsOn } from '../types';
export declare function getMonthName(date: Dayjs): string;
export declare function getDayHeaders(weekStartsOn: CalendarWeekStartsOn): string[];

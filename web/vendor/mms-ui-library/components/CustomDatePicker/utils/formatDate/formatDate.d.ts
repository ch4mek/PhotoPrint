import { Dayjs } from 'dayjs';
export declare const clampDateToRange: ([day, month, year]: string[], minDate?: Dayjs, maxDate?: Dayjs) => [string, string, string];
export declare const formatDate: ([day, month, year]: string[], minDate?: Dayjs, maxDate?: Dayjs, isArrowChange?: boolean) => string[];

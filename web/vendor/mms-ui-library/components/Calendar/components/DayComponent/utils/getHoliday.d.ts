import { Dayjs } from 'dayjs';
import { Holiday } from '@/components/Calendar/types';
export declare const getHoliday: (holidays: Holiday[], date: Dayjs) => Holiday | undefined;

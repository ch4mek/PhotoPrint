import { Dayjs } from 'dayjs';
import { Timestamp } from '@/types/date';
export declare const convertTimestampToDate: <TDefault = undefined>(timestamp: Timestamp | null | undefined, defaultValue?: TDefault) => Dayjs | TDefault;

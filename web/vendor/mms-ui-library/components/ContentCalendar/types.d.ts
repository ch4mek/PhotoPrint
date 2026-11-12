import { CSSObject } from '@emotion/react';
import { Dayjs } from 'dayjs';
export interface DayContentDataItem {
    date: string;
    content: string;
    background: string;
}
interface ContentCalendarStylesConfig {
    grid?: CSSObject;
    headerCell?: CSSObject;
    wrapper?: CSSObject;
}
export interface ContentCalendarProps {
    daysData: DayContentDataItem[];
    stylesConfig?: ContentCalendarStylesConfig;
    targetDate?: string | Date | Dayjs;
    fixedWeeksCount?: number;
}
export interface ContentCalendarDay {
    day: number;
    date: Dayjs;
    isCurrentMonth: boolean;
    isToday: boolean;
    isTrackedDay: boolean;
    content: string;
    background: string;
    weeksCount: number;
}
export interface GetSimpleCalendarParams {
    data?: DayContentDataItem[];
    targetDate?: string | Date | Dayjs;
    fixedWeeksCount?: number;
}
export {};

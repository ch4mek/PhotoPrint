import { ContentCalendarDay, GetSimpleCalendarParams } from '../types';
export declare const getSimpleCalendar: ({ data, targetDate, fixedWeeksCount, }?: GetSimpleCalendarParams) => {
    days: ContentCalendarDay[];
    weeksCount: number;
};

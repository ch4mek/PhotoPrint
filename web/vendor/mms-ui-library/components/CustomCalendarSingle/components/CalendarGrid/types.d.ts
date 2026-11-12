import { Dayjs } from 'dayjs';
import { BaseCalendarDay, RangeSelectionDay } from '../../types';
export interface CalendarGridProps {
    month?: Dayjs;
    days?: BaseCalendarDay[] | RangeSelectionDay[];
    isLeftCalendar?: boolean;
}

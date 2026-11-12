import { Dayjs } from 'dayjs';
import { CalendarConfigInput, CalendarNavigation, RangeCalendarPart } from '../../../CustomCalendarSingle/types';
export interface SingleCalendarProps {
    side: RangeCalendarPart;
    month: Dayjs;
    config: CalendarConfigInput;
    onNavigate: (direction: CalendarNavigation) => void;
    className?: string;
    isCalendarFitScreen?: boolean;
}

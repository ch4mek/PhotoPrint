import { Dayjs } from 'dayjs';
export interface MonthYearCalendarProps {
    value: Dayjs;
    onChange: (date: Dayjs) => void;
    minYear?: Dayjs;
    maxYear?: Dayjs;
    dataTestId?: string;
}
export interface CalendarWrapperProps {
    dataTestId?: string;
}

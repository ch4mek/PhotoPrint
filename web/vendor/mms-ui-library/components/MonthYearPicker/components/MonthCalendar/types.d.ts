import { Dayjs } from 'dayjs';
export interface MonthButtonProps {
    active: boolean;
}
export interface MonthCalendarProps {
    displayedYear: number;
    onYearClick: VoidFunction;
    onPrev: VoidFunction;
    onNext: VoidFunction;
    displayedMonth: Dayjs;
    onMonthSelect: (monthIndex: number) => void;
    dataTestId?: string;
}

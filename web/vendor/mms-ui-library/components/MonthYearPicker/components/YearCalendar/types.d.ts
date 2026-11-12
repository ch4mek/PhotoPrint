import { Dayjs } from 'dayjs';
export interface YearCalendarProps {
    minYear: Dayjs;
    maxYear: Dayjs;
    selectedYear: number;
    onYearSelect: (year: number) => void;
    dataTestId?: string;
}
export interface YearButtonProps {
    active: boolean;
}

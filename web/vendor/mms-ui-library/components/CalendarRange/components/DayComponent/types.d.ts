import { PickersDayProps } from '@mui/x-date-pickers';
import { Dayjs } from 'dayjs';
import { Holiday } from '../../types';
import { Timestamp } from '@/types/date';
export interface CreateDayComponentArgs {
    startDate: Timestamp | null;
    endDate: Timestamp | null;
    holidays: Holiday[];
    hideToday: boolean;
    isStatic: boolean;
    onClick?: (day: Dayjs) => void;
    hoverDate?: Dayjs | null;
    setHoverDate?: (day: Dayjs | null) => void;
}
export interface CustomPickerDayProps extends PickersDayProps<Dayjs> {
    isStart: boolean;
    isEnd: boolean;
    isRange: boolean;
    isAdjacent: boolean;
    isHoliday: boolean;
    isStatic: boolean;
    isHover?: boolean;
}

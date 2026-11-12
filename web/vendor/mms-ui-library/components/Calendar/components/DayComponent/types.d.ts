import { PickersDayProps } from '@mui/x-date-pickers';
import { Dayjs } from 'dayjs';
import { Holiday } from '../../types';
export interface CustomPickerDayProps extends PickersDayProps<Dayjs> {
    isHoliday?: boolean;
}
export interface CreateDayComponentArgs {
    holidays: Holiday[];
    hideToday: boolean;
    onClick?: (day: Dayjs) => void;
}

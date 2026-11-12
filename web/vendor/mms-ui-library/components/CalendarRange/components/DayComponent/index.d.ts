import { PickersDayProps } from '@mui/x-date-pickers';
import { Dayjs } from 'dayjs';
import { CreateDayComponentArgs } from './types';
export declare function createDayComponent({ startDate, endDate, holidays, hideToday, isStatic, onClick, hoverDate, setHoverDate, }: CreateDayComponentArgs): import("react").MemoExoticComponent<(props: PickersDayProps<Dayjs>) => import("react/jsx-runtime").JSX.Element>;

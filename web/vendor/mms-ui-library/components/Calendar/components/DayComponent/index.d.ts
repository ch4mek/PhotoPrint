import { PickersDayProps } from '@mui/x-date-pickers';
import { Dayjs } from 'dayjs';
import { CreateDayComponentArgs } from './types';
export declare function createDayComponent({ hideToday, holidays, onClick, }: CreateDayComponentArgs): import("react").MemoExoticComponent<(props: PickersDayProps<Dayjs>) => import("react/jsx-runtime").JSX.Element>;

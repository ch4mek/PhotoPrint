import { PickersCalendarHeaderProps } from '@mui/x-date-pickers';
import { Dayjs } from 'dayjs';
import { DropDownProps } from '@/components/Dropdown';
export interface InteractiveCalendarHeaderProps extends PickersCalendarHeaderProps<Dayjs> {
    monthDropdownProps?: Partial<Omit<DropDownProps, 'value' | 'options' | 'onChange'>>;
    yearDropdownProps?: Partial<Omit<DropDownProps, 'value' | 'options' | 'onChange'>>;
}

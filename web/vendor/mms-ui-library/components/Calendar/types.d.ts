import { Theme } from '@mui/system';
import { DateCalendarProps, DateView, LocalizationProviderProps } from '@mui/x-date-pickers';
import { PickerSelectionState } from '@mui/x-date-pickers/internals';
import { Dayjs } from 'dayjs';
import { DropDownProps } from '@/components/Dropdown/types';
import { Timestamp } from '@/types/date';
export interface Holiday {
    date: Dayjs;
    title: string;
}
export interface CalendarProps extends DateCalendarProps<Dayjs> {
    isInteractive: boolean;
    calendarWidth?: number;
    theme?: Theme;
}
export interface DatePickerProps {
    isInteractive?: boolean;
    localizationProviderProps?: LocalizationProviderProps<Dayjs, string>;
}
export type CalendarUIProps = DatePickerProps & Omit<DateCalendarProps<Dayjs>, 'value' | 'defaultValue' | 'minDate' | 'maxDate' | 'onChange'> & {
    holidays?: Holiday[];
    value?: Timestamp | null;
    defaultValue?: Timestamp | null;
    minDate?: Timestamp;
    maxDate?: Timestamp;
    monthDropdownProps?: Partial<Omit<DropDownProps, 'value' | 'options' | 'onChange'>>;
    yearDropdownProps?: Partial<Omit<DropDownProps, 'value' | 'options' | 'onChange'>>;
    onChange?: (value: Timestamp | null, selectionState?: PickerSelectionState, selectedView?: DateView | undefined) => void;
};

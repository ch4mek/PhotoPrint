import { CalendarsWrappersProps, CalendarWrapperProps } from './types';
export declare const CalendarsWrappers: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & CalendarsWrappersProps, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const LeftCalendar: import("@emotion/styled").StyledComponent<import("../Calendar").DatePickerProps & Omit<import("@mui/x-date-pickers").DateCalendarProps<import("dayjs").Dayjs>, "value" | "onChange" | "defaultValue" | "minDate" | "maxDate"> & {
    holidays?: import("../Calendar").Holiday[];
    value?: import("../../types/date").Timestamp | null;
    defaultValue?: import("../../types/date").Timestamp | null;
    minDate?: import("../../types/date").Timestamp;
    maxDate?: import("../../types/date").Timestamp;
    monthDropdownProps?: Partial<Omit<import("../Dropdown").DropDownProps, "value" | "options" | "onChange">>;
    yearDropdownProps?: Partial<Omit<import("../Dropdown").DropDownProps, "value" | "options" | "onChange">>;
    onChange?: (value: import("../../types/date").Timestamp | null, selectionState?: import("@mui/x-date-pickers/internals").PickerSelectionState, selectedView?: import("@mui/x-date-pickers").DateView | undefined) => void;
} & {
    theme?: import("@emotion/react").Theme;
} & CalendarWrapperProps, {}, {}>;
export declare const RightCalendar: import("@emotion/styled").StyledComponent<import("../Calendar").DatePickerProps & Omit<import("@mui/x-date-pickers").DateCalendarProps<import("dayjs").Dayjs>, "value" | "onChange" | "defaultValue" | "minDate" | "maxDate"> & {
    holidays?: import("../Calendar").Holiday[];
    value?: import("../../types/date").Timestamp | null;
    defaultValue?: import("../../types/date").Timestamp | null;
    minDate?: import("../../types/date").Timestamp;
    maxDate?: import("../../types/date").Timestamp;
    monthDropdownProps?: Partial<Omit<import("../Dropdown").DropDownProps, "value" | "options" | "onChange">>;
    yearDropdownProps?: Partial<Omit<import("../Dropdown").DropDownProps, "value" | "options" | "onChange">>;
    onChange?: (value: import("../../types/date").Timestamp | null, selectionState?: import("@mui/x-date-pickers/internals").PickerSelectionState, selectedView?: import("@mui/x-date-pickers").DateView | undefined) => void;
} & {
    theme?: import("@emotion/react").Theme;
} & CalendarWrapperProps, {}, {}>;

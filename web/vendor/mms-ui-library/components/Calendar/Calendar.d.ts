import { CalendarUIProps } from './types';
declare function Calendar({ value: valueTimestamp, defaultValue: defaultValueTimestamp, minDate: minDateTimestamp, maxDate: maxDateTimestamp, localizationProviderProps, onChange, holidays, isInteractive, monthDropdownProps, yearDropdownProps, ...props }: CalendarUIProps): import("react/jsx-runtime").JSX.Element;
export declare const CalendarUI: import("react").MemoExoticComponent<typeof Calendar>;
export {};

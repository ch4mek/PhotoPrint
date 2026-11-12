import { CalendarMode, CalendarWeekStartsOn } from '../types';
export declare function getSingleArgTypes(): {
    selectedDate: {
        control: {
            type: "date";
        };
        description: string;
    };
    calendarWidth: {
        control: {
            type: "number";
        };
        description: string;
    };
    showDaysOutsideCurrentMonth: {
        control: {
            type: "boolean";
        };
        description: string;
    };
    disablePast: {
        control: {
            type: "boolean";
        };
        description: string;
    };
    disableFuture: {
        control: {
            type: "boolean";
        };
        description: string;
    };
    isInteractive: {
        control: {
            type: "boolean";
        };
        description: string;
    };
    mode: {
        control: {
            type: "select";
        };
        options: CalendarMode[];
        description: string;
        table: {
            disable: boolean;
        };
    };
    onMonthChange: {
        control: {
            type: "object";
        };
        table: {
            disable: boolean;
        };
    };
    onYearChange: {
        control: {
            type: "object";
        };
        table: {
            disable: boolean;
        };
    };
    showWeekends: {
        control: {
            type: "boolean";
        };
        description: string;
    };
    showHolidays: {
        control: {
            type: "boolean";
        };
        description: string;
    };
    weekStartsOn: {
        control: {
            type: "select";
        };
        options: CalendarWeekStartsOn[];
        description: string;
    };
    minDate: {
        control: {
            type: "date";
        };
        description: string;
    };
    maxDate: {
        control: {
            type: "date";
        };
        description: string;
    };
    disabled: {
        control: {
            type: "boolean";
        };
        description: string;
    };
    disableHighlightToday: {
        control: {
            type: "boolean";
        };
        description: string;
    };
    holidays: {
        control: {
            type: "object";
        };
        description: string;
    };
    className: {
        control: {
            type: "text";
        };
        description: string;
    };
    onChange: {
        control: {
            type: "object";
        };
        table: {
            disable: boolean;
        };
    };
};
export declare function getRangeArgTypes(): {
    selectedRange: {
        control: {
            type: "object";
        };
        description: string;
    };
    defaultRange: {
        control: {
            type: "object";
        };
        description: string;
    };
    isStatic: {
        control: {
            type: "boolean";
        };
        description: string;
    };
    isInteractive: {
        table: {
            disable: boolean;
        };
    };
    disablePast: {
        table: {
            disable: boolean;
        };
    };
    disableFuture: {
        table: {
            disable: boolean;
        };
    };
    showDaysOutsideCurrentMonth: {
        table: {
            disable: boolean;
        };
    };
    onMonthChange: {
        table: {
            disable: boolean;
        };
    };
    onYearChange: {
        table: {
            disable: boolean;
        };
    };
    mode: {
        control: {
            type: "select";
        };
        options: CalendarMode[];
        description: string;
        table: {
            disable: boolean;
        };
    };
    showWeekends: {
        control: {
            type: "boolean";
        };
        description: string;
    };
    showHolidays: {
        control: {
            type: "boolean";
        };
        description: string;
    };
    weekStartsOn: {
        control: {
            type: "select";
        };
        options: CalendarWeekStartsOn[];
        description: string;
    };
    minDate: {
        control: {
            type: "date";
        };
        description: string;
    };
    maxDate: {
        control: {
            type: "date";
        };
        description: string;
    };
    disabled: {
        control: {
            type: "boolean";
        };
        description: string;
    };
    disableHighlightToday: {
        control: {
            type: "boolean";
        };
        description: string;
    };
    holidays: {
        control: {
            type: "object";
        };
        description: string;
    };
    className: {
        control: {
            type: "text";
        };
        description: string;
    };
    onChange: {
        control: {
            type: "object";
        };
        table: {
            disable: boolean;
        };
    };
};

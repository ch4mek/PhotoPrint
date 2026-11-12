import { CalendarMode } from '../types';
export declare const DURATION_PRESETS: {
    month: {
        minHeight: number;
        paddingTop: number;
        paddingBottom: number;
    };
    short: {
        minHeight: number;
        paddingTop: number;
        paddingBottom: number;
    };
    medium: {
        minHeight: number;
        paddingTop: number;
        paddingBottom: number;
    };
    long: {
        minHeight: number;
        paddingTop: number;
        paddingBottom: number;
    };
};
export declare const getEventSizeByDuration: (mode: CalendarMode, duration?: number) => {
    minHeight: number;
    paddingTop: number;
    paddingBottom: number;
};

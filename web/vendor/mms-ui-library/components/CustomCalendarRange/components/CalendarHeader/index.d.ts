import { Dayjs } from 'dayjs';
export declare const createLeftCalendarHeader: (onPrev: VoidFunction) => ({ currentMonth }: {
    currentMonth: Dayjs;
}) => import("react/jsx-runtime").JSX.Element;
export declare const createRightCalendarHeader: (onNext: VoidFunction) => ({ currentMonth }: {
    currentMonth: Dayjs;
}) => import("react/jsx-runtime").JSX.Element;
export declare const createStaticLeftCalendarHeader: () => ({ currentMonth }: {
    currentMonth: Dayjs;
}) => import("react/jsx-runtime").JSX.Element;
export declare const createStaticRightCalendarHeader: () => ({ currentMonth }: {
    currentMonth: Dayjs;
}) => import("react/jsx-runtime").JSX.Element;

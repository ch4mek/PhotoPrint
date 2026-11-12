interface Time {
    hours: number;
    minutes: number;
}
export declare const clampCycle: (value: number, min: number, max: number) => number;
export declare const parseTime: (value: string) => Time;
export declare const formatTime: ({ hours, minutes }: Time) => string;
export declare const isArrowUp: (key: string) => key is "ArrowUp";
export declare const isArrowDown: (key: string) => key is "ArrowDown";
export declare const getSelectionRange: (isHours: boolean) => [number, number];
export {};

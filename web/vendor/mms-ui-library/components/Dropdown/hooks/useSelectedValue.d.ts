type UseSelectedValueType = <T, K extends keyof T>(options: T[], value: T, valueField?: K) => T;
export declare const useSelectedValue: UseSelectedValueType;
export {};

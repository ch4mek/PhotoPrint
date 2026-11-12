import type { CounterProps } from './types';
declare function CounterInner({ value, onChange, minValue, maxValue, error, precision, step, dataTestId, disabled, editable, }: CounterProps): import("react/jsx-runtime").JSX.Element;
export declare const Counter: import("react").MemoExoticComponent<typeof CounterInner>;
export {};

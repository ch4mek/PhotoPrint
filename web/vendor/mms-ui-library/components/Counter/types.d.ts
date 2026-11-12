export type CounterAction = 'increment' | 'decrement';
export interface CounterProps {
    value: number;
    onChange: (value: number) => void;
    minValue?: number;
    maxValue?: number;
    error?: boolean;
    precision?: number;
    step?: number;
    editable?: boolean;
    disabled?: boolean;
    dataTestId?: string;
}
export type CounterButtonProps = {
    buttonType: CounterAction;
};
export type CounterWrapperProps = {
    disabled?: boolean;
    error?: boolean;
    precision?: number;
};

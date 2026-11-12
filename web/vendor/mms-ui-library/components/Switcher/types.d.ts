export interface SwitcherOption {
    value: string;
    [key: string]: string;
}
export interface SwitcherProps<T extends SwitcherOption> {
    value: T['value'];
    options: T[];
    onChange: (value: T['value']) => void;
    nameField?: keyof T;
    dataTestId?: string;
}
export interface SwitcherWrapperProps {
    dataTestId: string;
    optionsCount: number;
}
export interface SwitcherButtonProps {
    active: boolean;
}
export interface ActiveSliderProps {
}

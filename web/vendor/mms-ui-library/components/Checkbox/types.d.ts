import type { ChangeEvent, CSSProperties, InputHTMLAttributes, ReactNode } from 'react';
type NativeProps = InputHTMLAttributes<HTMLInputElement>;
export interface CheckboxProps extends Omit<NativeProps, 'onChange' | 'type'> {
    /** Id for interaction with label */
    id?: string;
    /** Text for checkbox */
    label?: ReactNode;
    /** State of cheked */
    checked?: boolean;
    /** Id for test */
    dataTestId?: string;
    /** Show hidden input */
    hiddenInput?: boolean;
    /** Use default marker values */
    defaultCheckMark?: boolean;
    /** State change handler */
    onChange?: CheckboxOnChangeType;
    /** Make the checkbox disabled */
    disabled?: boolean;
    /** Styles for checkbox wrapper */
    wrapperStyles?: CSSProperties;
    textCut?: boolean;
}
export type CheckboxOnChangeType = (event: ChangeEvent<HTMLInputElement>, payload: {
    checked: boolean;
    name?: string;
}) => void;
export type HiddenInputProps = {
    hidden: boolean;
};
export type WrapperProps = {
    disabled?: boolean;
    wrapperStyles?: CSSProperties;
};
export {};

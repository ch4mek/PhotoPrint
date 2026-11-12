import { InputHTMLAttributes, ChangeEvent } from 'react';
export type SwitchProps = Omit<InputHTMLAttributes<HTMLInputElement>, 'type' | 'onChange' | 'disabled'> & StyledSwitchProps & {
    dataTestId?: string;
    onChange?: (event: ChangeEvent<HTMLInputElement>, payload: {
        checked: boolean;
        name: InputHTMLAttributes<HTMLInputElement>['name'];
    }) => void;
};
export interface StyledSwitchProps {
    disabled?: boolean;
    checked?: boolean;
}
export interface StyledLabelProps {
    disabled?: boolean;
}

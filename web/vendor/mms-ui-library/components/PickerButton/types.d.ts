import { Placement } from '@floating-ui/react';
import { ButtonHTMLAttributes, Dispatch, ReactNode, RefAttributes, SetStateAction } from 'react';
export type PickerButtonView = 'outlined' | 'transparent';
export interface PickerButtonOptionProps {
    displayOnlyAddon?: boolean;
    removeOptionPaddings?: boolean;
}
export interface PickerButtonOption<T = never> {
    value: string;
    addon?: ReactNode;
    onClick?(item?: T): void;
}
export interface PickerButtonWrapperProps {
    view: PickerButtonView;
    disabled: boolean;
    isOpen: boolean;
}
export interface PickerButtonProps<T = never> extends Partial<Omit<PickerButtonWrapperProps, 'isOpen'>>, PickerButtonOptionProps, Omit<ButtonHTMLAttributes<HTMLButtonElement>, 'onClick' | 'onChange'>, RefAttributes<HTMLButtonElement> {
    icon: ReactNode;
    isOpenPopover?: boolean;
    item?: T;
    popoverPosition?: Placement;
    options: PickerButtonOption<T>[];
    onChange?(option: PickerButtonOption<T>): void;
    setIsOpenPopover?: Dispatch<SetStateAction<boolean>>;
    dataTestId?: string;
}

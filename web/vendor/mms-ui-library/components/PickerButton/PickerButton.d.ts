import { ForwardedRef } from 'react';
import { PickerButtonProps } from './types';
declare function PickerButtonInner<T = never>({ item, icon, type, view, disabled, popoverPosition, displayOnlyAddon, options, onChange, dataTestId, isOpenPopover: isOpenOuter, setIsOpenPopover: setIsOpenOuter, ref, removeOptionPaddings, ...otherProps }: PickerButtonProps<T>): import("react/jsx-runtime").JSX.Element;
export declare const PickerButton: <T>(props: PickerButtonProps<T> & {
    ref?: ForwardedRef<HTMLButtonElement>;
}) => ReturnType<typeof PickerButtonInner>;
export {};

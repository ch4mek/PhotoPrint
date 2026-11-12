import { ChangeEvent, CSSProperties, HTMLProps, RefAttributes } from 'react';
export interface SearchInputProps extends Omit<HTMLProps<HTMLInputElement>, 'size' | 'as' | 'ref'>, RefAttributes<HTMLInputElement> {
    value: string;
    size?: keyof SearchInputSizes;
    onChange?: (value: ChangeEvent<HTMLInputElement>) => void;
    stylesConfig?: SearchInputStylesConfig;
    showToolTipIfCut?: boolean;
    dataTestId?: string;
    placeholder?: string;
    hintText?: string;
    onDeleteIconClick?: VoidFunction;
    error?: boolean;
}
export interface SearchInputSizes {
    auto: string;
    fullWidth: string;
}
interface SearchInputStylesConfig {
    input?: CSSProperties;
    typography?: CSSProperties;
    placeholder?: CSSProperties;
}
export interface SearchInputFieldProps {
    stylesConfig?: SearchInputStylesConfig;
    disabled?: boolean;
    isFocused?: boolean;
}
export interface SearchInputWrapperProps {
    sizeField: keyof SearchInputSizes;
    disabled?: boolean;
    error?: boolean;
}
export {};

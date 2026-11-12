interface ErrorTextWrapperStyles {
    background?: string;
}
export interface ErrorTextStyles {
    wrapper?: ErrorTextWrapperStyles;
}
export interface ErrorTextProps {
    dataTestId: string;
    text?: string;
    stylesConfig?: ErrorTextStyles;
    isErrorIconVisible?: boolean;
}
export interface ErrorIconWrapperProps {
    stylesConfig?: ErrorTextWrapperStyles;
}
export {};

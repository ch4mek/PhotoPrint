import { InputFieldProps, LabelProps, StyledInputProps } from './types';
export declare const InputWrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & StyledInputProps, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const InputFieldWrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const InputField: import("@emotion/styled").StyledComponent<(((Omit<import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement> & {
    mask: DateConstructor;
} & Partial<Pick<import("imask").default.Masked<DateConstructor>, "eager" | "parent" | "prepare" | "validate" | "commit" | "overwrite" | "skipInvalid">> & Partial<Pick<import("imask").default.MaskedPattern<DateConstructor>, "lazy" | "blocks" | "definitions" | "placeholderChar" | "displayChar">> & Partial<Pick<import("imask").default.MaskedDate, "pattern" | "max" | "min" | "format" | "parse" | "autofix">> & import("react-imask/dist/mixin").ReactMaskProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement> & {
    ref?: React.Ref<React.ComponentType<import("react-imask/dist/mixin").IMaskInputProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement, import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement>>>>;
}, "ref"> | Omit<import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement> & {
    mask: NumberConstructor;
} & Partial<Pick<import("imask").default.Masked<NumberConstructor>, "eager" | "parent" | "prepare" | "validate" | "commit" | "overwrite" | "skipInvalid">> & Partial<Pick<import("imask").default.MaskedNumber, "scale" | "max" | "min" | "radix" | "thousandsSeparator" | "mapToRadix" | "signed" | "normalizeZeros" | "padFractionalZeros">> & import("react-imask/dist/mixin").ReactMaskProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement> & {
    ref?: React.Ref<React.ComponentType<import("react-imask/dist/mixin").IMaskInputProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement, import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement>>>>;
}, "ref"> | Omit<import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement> & {
    mask: string;
} & Partial<Pick<import("imask").default.Masked<string>, "eager" | "parent" | "prepare" | "validate" | "commit" | "overwrite" | "skipInvalid">> & Partial<Pick<import("imask").default.MaskedPattern<string>, "lazy" | "blocks" | "definitions" | "placeholderChar" | "displayChar">> & import("react-imask/dist/mixin").ReactMaskProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement> & {
    ref?: React.Ref<React.ComponentType<import("react-imask/dist/mixin").IMaskInputProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement, import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement>>>>;
}, "ref"> | Omit<import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement> & {
    mask: import("imask").default.AnyMaskedOptionsArray;
} & Partial<Pick<import("imask").default.Masked<import("imask").default.AnyMaskedOptionsArray>, "eager" | "parent" | "prepare" | "validate" | "commit" | "overwrite" | "skipInvalid">> & Partial<Pick<import("imask").default.MaskedDynamic, "dispatch">> & import("react-imask/dist/mixin").ReactMaskProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement> & {
    ref?: React.Ref<React.ComponentType<import("react-imask/dist/mixin").IMaskInputProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement, import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement>>>>;
}, "ref"> | Omit<import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement> & {
    mask: RegExp;
} & Partial<Pick<import("imask").default.Masked<RegExp>, "eager" | "parent" | "prepare" | "validate" | "commit" | "overwrite" | "skipInvalid">> & import("react-imask/dist/mixin").ReactMaskProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement> & {
    ref?: React.Ref<React.ComponentType<import("react-imask/dist/mixin").IMaskInputProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement, import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement>>>>;
}, "ref"> | Omit<import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement> & {
    mask: Function;
} & Partial<Pick<import("imask").default.Masked<Function>, "eager" | "parent" | "prepare" | "validate" | "commit" | "overwrite" | "skipInvalid">> & import("react-imask/dist/mixin").ReactMaskProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement> & {
    ref?: React.Ref<React.ComponentType<import("react-imask/dist/mixin").IMaskInputProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement, import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement>>>>;
}, "ref"> | Omit<import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement> & Record<string, any> & import("react-imask/dist/mixin").ReactMaskProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement> & {
    ref?: React.Ref<React.ComponentType<import("react-imask/dist/mixin").IMaskInputProps<import("imask").default.AnyMaskedOptions, boolean | "typed", unknown, HTMLInputElement, import("react-imask/dist/mixin").ReactElementProps<HTMLInputElement>>>>;
}, "ref">) & import("react").RefAttributes<unknown>) & {
    theme?: import("@emotion/react").Theme;
}) & InputFieldProps, {}, {}>;
export declare const AdornmentWrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const Label: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & LabelProps, import("react").DetailedHTMLProps<import("react").LabelHTMLAttributes<HTMLLabelElement>, HTMLLabelElement>, {}>;
export declare const HelperText: import("@emotion/styled").StyledComponent<import("../Typography").TypographyProps & {
    theme?: import("@emotion/react").Theme;
}, {}, {}>;

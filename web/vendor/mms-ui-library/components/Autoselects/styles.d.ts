import type { BodyFontSize } from '@/types/emotion';
import type { ChipsContainerProps, ContainerProps, LabelProps, WrapperProps } from './types';
export declare const Wrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & WrapperProps, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const Container: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & ContainerProps, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const ScrollWrapper: import("@emotion/styled").StyledComponent<(import("../Scrollbar").ScrollbarProps & {
    theme?: import("@emotion/react").Theme;
}) & {}, {}, {}>;
export declare const ChipsContainer: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & ChipsContainerProps, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const Input: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    inputTypography: BodyFontSize;
}, import("react").DetailedHTMLProps<import("react").InputHTMLAttributes<HTMLInputElement>, HTMLInputElement>, {}>;
export declare const ButtonsWrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const CrossButtonBox: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const CrossButton: import("@emotion/styled").StyledComponent<{
    children?: import("react").ReactNode | undefined;
} & Partial<import("../ActionButton").ActionButtonWrapperProps> & import("react").ButtonHTMLAttributes<HTMLButtonElement> & {
    theme?: import("@emotion/react").Theme;
}, {}, {}>;
export declare const ArrowButton: import("@emotion/styled").StyledComponent<{
    children?: import("react").ReactNode | undefined;
} & Partial<import("../ActionButton").ActionButtonWrapperProps> & import("react").ButtonHTMLAttributes<HTMLButtonElement> & {
    theme?: import("@emotion/react").Theme;
}, {}, {}>;
export declare const Label: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & LabelProps, import("react").DetailedHTMLProps<import("react").LabelHTMLAttributes<HTMLLabelElement>, HTMLLabelElement>, {}>;
export declare const HintInner: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const Indicator: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const CreateRow: import("@emotion/styled").StyledComponent<import("@/components/Typography").TypographyProps & {
    theme?: import("@emotion/react").Theme;
}, {}, {}>;

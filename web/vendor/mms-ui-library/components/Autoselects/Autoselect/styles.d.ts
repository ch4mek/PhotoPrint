import { StylesConfig } from '../types';
import type { BodyFontSize } from '@/types/emotion';
export declare const Container: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    height: string;
    disabled: boolean;
    stylesConfig?: Omit<StylesConfig, "typography">;
    borderColor: "graphics-accent" | "graphics-error" | "graphics-primary-2" | "graphics-input-stroke";
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const InputContainer: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    height: string;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const Input: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    showEllipsis: boolean;
    inputTypography: BodyFontSize;
    height: string;
}, import("react").DetailedHTMLProps<import("react").InputHTMLAttributes<HTMLInputElement>, HTMLInputElement>, {}>;

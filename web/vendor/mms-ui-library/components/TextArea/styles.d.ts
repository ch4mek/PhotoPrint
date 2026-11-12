import type { ContainerProps, BordersProps, InputFieldProps } from './types';
export declare const Container: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & ContainerProps, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const SpeakerphoneWrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    isHasAdornment: boolean;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const Border: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & BordersProps, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const TextAreaFieldWrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    isHasAdornment: boolean;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const TextAreaField: import("@emotion/styled").StyledComponent<import("react-textarea-autosize").TextareaAutosizeProps & import("react").RefAttributes<HTMLTextAreaElement> & {
    theme?: import("@emotion/react").Theme;
} & InputFieldProps, {}, {}>;

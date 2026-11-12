import { CalendarMode } from '../types';
export declare const Container: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    hasIcon?: boolean;
    isActive?: boolean;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const EventWrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    mode: CalendarMode;
    active?: boolean;
    duration?: number;
    isFullWidth?: boolean;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const Text: import("@emotion/styled").StyledComponent<import("@/components/Typography").TypographyProps & {
    theme?: import("@emotion/react").Theme;
}, {}, {}>;
export declare const Content: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    active?: boolean;
    duration?: number;
    mode: CalendarMode;
    hasIcon?: boolean;
    showActiveIcon?: boolean;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const OrganizerInfoBlock: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const OrganizerIcon: import("@emotion/styled").StyledComponent<import("react").SVGProps<SVGSVGElement> & {
    theme?: import("@emotion/react").Theme;
}, {}, {}>;
export declare const ActionsContainer: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const CallButton: import("@emotion/styled").StyledComponent<{
    children?: import("react").ReactNode | undefined;
} & Partial<import("@/components/ActionButton").ActionButtonWrapperProps> & import("react").ButtonHTMLAttributes<HTMLButtonElement> & {
    theme?: import("@emotion/react").Theme;
}, {}, {}>;
export declare const IconWrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    mode: CalendarMode;
    isActive: boolean;
    eventDuration: number;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;

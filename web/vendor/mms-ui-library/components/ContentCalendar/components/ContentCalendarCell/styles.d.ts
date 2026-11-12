export declare const ContentTypography: import("@emotion/styled").StyledComponent<import("@/components/Typography").TypographyProps & {
    theme?: import("@emotion/react").Theme;
}, {}, {}>;
export declare const CellContent: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const CellDayDate: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    isToday: boolean;
    isTrackedDay?: boolean;
    background?: string;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const CalendarGridCell: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    isCurrentMonth: boolean;
    isTrackedDay?: boolean;
    background?: string;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;

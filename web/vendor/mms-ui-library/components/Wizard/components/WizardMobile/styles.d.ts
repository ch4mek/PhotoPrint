import { WizardStylesConfig } from '../../types';
export declare const WizardMobileWrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    styles?: WizardStylesConfig["header"];
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const WizardMobileHeaderWrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const WizardMobileHeaderTitleText: import("@emotion/styled").StyledComponent<import("@/components/Typography").TypographyProps & {
    theme?: import("@emotion/react").Theme;
}, {}, {}>;
export declare const WizardMobileHeaderStepText: import("@emotion/styled").StyledComponent<import("@/components/Typography").TypographyProps & {
    theme?: import("@emotion/react").Theme;
}, {}, {}>;
export declare const WizardMobileHeaderActiveStepText: import("@emotion/styled").StyledComponent<import("@/components/Typography").TypographyProps & {
    theme?: import("@emotion/react").Theme;
}, {}, {}>;
export declare const WizardMobileProgressBaseLine: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const WizardMobileProgressFill: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    stepsCount: number;
    activeStep: number;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;

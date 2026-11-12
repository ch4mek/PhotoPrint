import { CSSObject } from '@emotion/styled';
import { ReactNode } from 'react';
export declare const enum StepStatus {
    default = "default",
    active = "active"
}
export interface Step {
    id: number;
    title: string;
    component: ReactNode;
    validator: () => Promise<boolean>;
}
export interface WizardStylesConfig {
    header?: CSSObject;
    container?: CSSObject;
    content?: CSSObject;
    button?: CSSObject;
}
export interface WizardProps {
    steps: Step[];
    isMobileView?: boolean;
    dataTestId?: string;
    stylesConfig?: WizardStylesConfig;
    onComplete?: VoidFunction;
    onLoading?: (isLoading: boolean) => void;
    onStepChange?: (step: number) => void;
}

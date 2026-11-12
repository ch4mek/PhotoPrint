import { Step, WizardStylesConfig } from '../../types';
export interface WizardMobileProps {
    steps: Step[];
    completedSteps: Set<number>;
    activeStep: number;
    stylesConfig?: WizardStylesConfig;
}

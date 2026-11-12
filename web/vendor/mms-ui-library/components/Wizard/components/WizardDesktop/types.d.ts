import { Step, WizardStylesConfig } from '../../types';
export interface WizardDesktopProps {
    steps: Step[];
    activeStep: number;
    completedSteps: Set<number>;
    onNavigateToStep: (stepToNavigate: number) => void;
    stylesConfig?: WizardStylesConfig;
}

import { StepStatus } from '../../types';
export interface WizardStepProps {
    stepNumber: number;
    title: string;
    status: StepStatus;
    dataTestId?: string;
    onClick?: VoidFunction;
}

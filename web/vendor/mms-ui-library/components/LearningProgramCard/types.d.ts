import { ReactNode } from 'react';
import { LearningProgramStatusProps } from './components';
export type LearningProgramCardProps = {
    title: string;
    summary: string;
    coursesLabel: string;
    departmentLabel: string;
    departmentIcon: ReactNode;
    onClick?: VoidFunction;
    dataTestId?: string;
} & (LearningProgramStatusProps | Partial<Record<keyof LearningProgramStatusProps, undefined>>);
export { LearningProgramStatus } from './components';

import { ReactNode } from 'react';
export interface StackProps {
    children: (value: number) => ReactNode;
    value?: number;
}

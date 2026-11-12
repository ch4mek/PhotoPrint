import { RefAttributes } from 'react';
export declare enum LoaderVariant {
    Determinate = "determinate",
    Indeterminate = "indeterminate"
}
type Width = number | '100%';
export interface LinearLoaderProps extends RefAttributes<HTMLDivElement> {
    variant?: LoaderVariant;
    value?: number;
    dataTestId?: string;
    width?: Width;
}
export interface BodyProps {
    width?: Width;
}
export interface InnerProps {
    width: string;
}
export {};

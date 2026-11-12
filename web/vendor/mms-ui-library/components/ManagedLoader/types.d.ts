import { CSSProperties, PropsWithChildren } from 'react';
import { SpinnerSize } from '../Spinner/types';
type LoaderType = 'hard' | 'soft';
export type ManagedLoaderProps = PropsWithChildren & {
    isLoading: boolean;
    size?: SpinnerSize;
    dataTestId?: string;
    loaderType?: LoaderType;
    height?: number;
    styles?: CSSProperties;
    displayDelay?: number;
};
export type LoaderContextValues = {
    loadingCount: number;
    isLoading: boolean;
    startLoading?: (entryName: string) => void;
    stopLoading?: (entryName: string) => void;
};
export type LoaderWrapperProps = {
    isHidden?: boolean;
    height?: number;
    styles?: CSSProperties;
};
export {};

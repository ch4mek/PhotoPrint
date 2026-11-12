import { HTMLAttributes } from 'react';
export interface SkeletonProps extends HTMLAttributes<HTMLDivElement> {
    visible: boolean;
    dataTestId?: string;
    width?: string | number;
    height?: string | number;
    skeletonElementProps?: HTMLAttributes<HTMLDivElement>;
}

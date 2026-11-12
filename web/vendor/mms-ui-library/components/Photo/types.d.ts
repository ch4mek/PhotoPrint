import { MouseEvent } from 'react';
export type PhotoProps = {
    firstName: string;
    lastName: string;
    src?: string;
    color?: string;
    bgColor?: string;
    isEditable?: boolean;
    onClick?: (e: MouseEvent<HTMLDivElement>) => void;
    onMouseEnter?: (e: MouseEvent<HTMLDivElement>) => void;
    onMouseLeave?: (e: MouseEvent<HTMLDivElement>) => void;
    size?: number;
    dataTestId?: string;
};
export type CircleProps = {
    color?: string;
    bgColor?: string;
    isEditable?: boolean;
    src?: string;
    size: number;
};

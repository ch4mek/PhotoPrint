import { ReactNode } from 'react';
export interface NoteProps {
    children: ReactNode;
    bgColor?: string;
    textColor?: string;
    iconStyles?: IconStyles;
    dataTestId?: string;
}
interface IconStyles {
    circleColor?: string;
    signColor?: string;
}
export interface DefaultComponentProps {
    color?: string;
}
export {};

import { CSSProperties, HTMLAttributes, ReactNode, RefAttributes } from 'react';
export type WrapperProps = {
    stylesConfig?: CSSProperties;
};
export type TagProps = HTMLAttributes<HTMLDivElement> & RefAttributes<HTMLDivElement> & {
    children?: ReactNode;
    disabled?: boolean;
    stylesConfig?: CSSProperties;
    onClick?: VoidFunction;
    dataTestId?: string;
};

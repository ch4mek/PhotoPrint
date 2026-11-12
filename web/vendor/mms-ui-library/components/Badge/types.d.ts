import { SerializedStyles } from '@emotion/react';
export type BadgeColor = 'primary' | 'secondary';
export type BadgeVariant = 'dot' | 'standard';
export interface BadgeProps {
    children: React.ReactNode;
    badgeContent?: number | string;
    color?: BadgeColor;
    invisible?: boolean;
    max?: number;
    showZero?: boolean;
    variant?: BadgeVariant;
}
export type BadgeSpanProps = {
    colorVariant: BadgeColor;
    variant: BadgeVariant;
};
export type BadgeColorStyles = {
    [colorKey in BadgeColor]: SerializedStyles;
};

import type { ElementType, HTMLAttributes, PropsWithChildren, RefAttributes } from 'react';
import type { BodyFontSize, FontSize } from '@/types/emotion';
type TypographyAlignVariants = 'center' | 'inherit' | 'justify' | 'left' | 'right';
export type AvailableComponentType = Extract<ElementType, 'p' | 'span' | 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6' | 'a'>;
export type TypographyVariants = FontSize | BodyFontSize;
export interface TypographyProps extends PropsWithChildren, HTMLAttributes<HTMLSpanElement>, RefAttributes<HTMLSpanElement> {
    align?: TypographyAlignVariants;
    noWrap?: boolean;
    countOfLines?: number;
    Component?: AvailableComponentType;
    fullWidth?: boolean;
    variant: TypographyVariants;
    color?: string;
    defaultMargin?: boolean;
    margin?: number;
    href?: string;
    dataTestId?: string;
}
export {};

import { spaces } from '@/theme';
export interface GapProps {
    size: keyof typeof spaces | number;
    direction?: 'horizontal' | 'vertical';
    tag?: 'div' | 'span';
    className?: string;
    dataTestId?: string;
}
export interface GapWrapperProps {
    size: keyof typeof spaces | number;
    direction?: 'horizontal' | 'vertical';
}

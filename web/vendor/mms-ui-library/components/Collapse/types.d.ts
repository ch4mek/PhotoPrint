import { CSSProperties, HTMLAttributes, ReactNode, TransitionEvent as ReactTransitionEvent } from 'react';
import type { CollapseGroupWrapperProps, Option, RenderCollapseItemFunction } from '../CollapseGroup/types';
export interface CollapseProps<U extends Option> extends HTMLAttributes<HTMLElement> {
    item: U;
    hasTopLevelStyles?: boolean;
    children?: ReactNode;
    expandIcon?: ReactNode;
    collapseIcon?: ReactNode;
    expanded?: boolean;
    fullwidth?: boolean;
    disabled?: boolean;
    unMountOnExit?: boolean;
    timeout?: number;
    maxWidth?: number;
    easing?: string;
    contentStyles?: CSSProperties;
    onTransitionEnd?: (event: ReactTransitionEvent<HTMLElement>) => void;
    renderHeader?: RenderCollapseItemFunction<U>;
    collapseGroupWrapper?: <T>({ children, hasTopLevelStyles, item, ...restProps }: CollapseGroupWrapperProps<U> & HTMLAttributes<T>) => ReactNode;
    toggleCollapse?: (item: U) => void;
}

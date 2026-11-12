import { ReactNode } from 'react';
import { BaseGroupNavigationItemProps, DefaultHandlers } from './components';
export type NestedType<T = unknown> = BaseGroupNavigationItemProps<T> & {
    children?: NestedType<T>[];
};
export type GroupNavigationProps<T = unknown> = DefaultHandlers & {
    items: NestedType<T>[];
    isFullWidth?: boolean;
    dataTestId?: string;
    renderAdditionalContent?: (id: string, additionalContentParams?: T) => ReactNode;
};

import { HTMLAttributes, MouseEvent, MouseEventHandler, ReactNode } from 'react';
export type RenderCollapseItemFunction<U> = <T extends HTMLElement>({ disabled, expanded, children, item, ...restProps }: {
    onClick: MouseEventHandler<T>;
    disabled: boolean;
    expanded?: boolean;
    item: U;
    children?: ReactNode;
} & HTMLAttributes<T>) => ReactNode;
export type RenderStaticItemFunction<U> = <T extends HTMLElement>({ children, item, }: {
    disabled: boolean;
    item: U;
    onClick?: MouseEventHandler<T>;
    children?: ReactNode;
} & HTMLAttributes<T>) => ReactNode;
export interface CollapseGroupWrapperProps<T extends Option> {
    children: ReactNode;
    hasTopLevelStyles: boolean;
    item: T;
}
export interface CollapseGroupProps<U extends Option> {
    options: U[];
    disabled?: boolean;
    hasTopLevelStyles?: boolean;
    unMountOnExit?: boolean;
    onContentClick?: (event: MouseEvent<HTMLElement>, payload: U) => void;
    renderCollapseItem?: RenderCollapseItemFunction<U>;
    renderStaticItem?: RenderStaticItemFunction<U>;
    staticItemWrapper?: <T>({ children, ...restProps }: {
        children: ReactNode;
        parentGroupId?: number | null;
    } & HTMLAttributes<T>) => ReactNode;
    collapseGroupWrapper?: <T>({ children, hasTopLevelStyles, item, ...restProps }: CollapseGroupWrapperProps<U> & HTMLAttributes<T>) => ReactNode;
    toggleCollapse?: (item: U) => void;
}
export interface Option {
    id: number | string;
    isGroup: boolean;
    name: string;
    parentGroupId?: number | null | string;
    expanded?: boolean;
    items?: Option[];
}

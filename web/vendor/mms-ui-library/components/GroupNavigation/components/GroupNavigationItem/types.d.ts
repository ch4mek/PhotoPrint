import { ComponentType, ReactNode, SVGProps } from 'react';
export declare enum GroupNavigationItemState {
    Empty = 0,
    Filled = 1
}
export declare enum GroupNavigationItemNesting {
    First = 1,
    Second = 2,
    Third = 3
}
export type BaseGroupNavigationItemProps<T = unknown> = {
    id: string;
    label: string;
    state: GroupNavigationItemState;
    nesting: GroupNavigationItemNesting;
    Icon?: ComponentType<SVGProps<SVGSVGElement>>;
    additionalContentParams?: T;
};
type DefaultEventHandler = (id: string) => void;
export type DefaultHandlers = {
    onSelect?: DefaultEventHandler;
    onToggle?: DefaultEventHandler;
    onFill?: DefaultEventHandler;
};
export type GroupNavigationItemProps<T = unknown> = BaseGroupNavigationItemProps<T> & DefaultHandlers & {
    children?: ReactNode;
    dataTestId?: string;
    renderAdditionalContent?: (id: string, additionalContentParams?: T) => ReactNode;
};
export {};

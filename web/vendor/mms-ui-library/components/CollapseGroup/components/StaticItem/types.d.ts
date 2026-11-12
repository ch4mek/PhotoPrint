import { HTMLAttributes, MouseEventHandler, ReactNode } from 'react';
import { Option, RenderStaticItemFunction } from '@/components/CollapseGroup/types';
export interface StaticItemProps<U extends Option> {
    item: U;
    disabled: boolean;
    renderStaticItem?: RenderStaticItemFunction<U>;
    onClick?: MouseEventHandler<HTMLElement>;
    staticItemWrapper?: <T>({ children, ...restProps }: {
        children: ReactNode;
    } & HTMLAttributes<T>) => ReactNode;
}

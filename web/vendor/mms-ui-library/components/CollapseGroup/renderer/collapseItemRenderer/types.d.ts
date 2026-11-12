import { MouseEventHandler } from 'react';
import { Option, RenderCollapseItemFunction } from '@/components/CollapseGroup/types';
export interface CollapseItemProps<T extends Option> {
    renderCollapseItem?: RenderCollapseItemFunction<T>;
    item: T;
}
export interface CollapseFunctionProps {
    onClick: MouseEventHandler<HTMLElement>;
    disabled: boolean;
    expanded?: boolean;
}

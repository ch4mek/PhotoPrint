import { MouseEventHandler, ReactNode } from 'react';
import { CollapseProps } from '../../types';
import { Option } from '@/components/CollapseGroup/types';
export interface CollapseHeaderProps<T extends Option> extends Pick<CollapseProps<T>, 'expanded' | 'disabled' | 'expandIcon' | 'collapseIcon'> {
    onClick: MouseEventHandler<HTMLElement>;
    children?: ReactNode;
}

import { ReactNode } from 'react';
import { BaseEventProps } from '../types';
export type EventProps = BaseEventProps & {
    isActive: boolean;
    children?: ReactNode | ReactNode[];
    onEventClick?: (id: string) => void;
    onCallClick?: (id: string) => void;
    dataTestId?: string;
    className?: string;
    isFullWidth?: boolean;
    icon?: ReactNode;
    showActiveIcon?: boolean;
};

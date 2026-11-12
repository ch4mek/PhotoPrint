import { PropsWithChildren, RefObject } from 'react';
import { TabsProps } from '../../types';
export type TabControlDirection = 'left' | 'right';
export interface TabsControlsProps extends Pick<TabsProps, 'isTopCornersRound' | 'disableVerticalScroll'>, PropsWithChildren {
    wrapperRef: RefObject<HTMLDivElement | null>;
    isShown: boolean;
    dataTestId: string;
}
export interface TabsControlProps {
    direction: TabControlDirection;
    disabled: boolean;
}
export type TabsControlsWrapperProps = Pick<TabsProps, 'isTopCornersRound'>;

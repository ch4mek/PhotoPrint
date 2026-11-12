import { RefCallback } from 'react';
import type { TypographyVariants } from '../Typography';
export interface SingleTab {
    id: number;
    text: string;
    disabled?: boolean;
}
export interface TabsWrapperProps {
    fullWidth?: boolean;
    isTopCornersRound: boolean;
    isOverflowing: boolean;
}
export interface TabProps extends SingleTab {
    active: boolean;
    fullWidth?: boolean;
    tabTypography?: TypographyVariants;
    onChangeTab: VoidFunction;
    customTabInnerPadding?: number;
    customActiveTabColor?: string;
    dataTestId: string;
    disableVerticalScroll?: boolean;
    ref: RefCallback<HTMLDivElement | null>;
}
export interface TabsProps extends Omit<TabProps, 'onChangeTab' | 'active' | 'id' | 'text' | 'disabled' | 'dataTestId' | 'ref'> {
    data: SingleTab[];
    activeTab: number;
    isTopCornersRound?: boolean;
    onChangeTab: (id: number) => void;
    dataTestId?: string;
}

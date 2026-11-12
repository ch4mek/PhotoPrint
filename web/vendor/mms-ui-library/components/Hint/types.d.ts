import { Placement } from '@floating-ui/react';
import { CSSProperties, ReactNode, RefAttributes } from 'react';
type RefElement = HTMLElement | null;
export interface HintProps extends RefAttributes<HTMLDivElement> {
    open: boolean;
    anchorElement: RefElement;
    position?: Placement;
    header?: ReactNode;
    text?: ReactNode;
    distanceToAnchorElement?: number;
    fallbackPlacements?: Array<Placement>;
    width?: number;
    useAnchorWidth?: boolean;
    dataTestId?: string;
    centered?: boolean;
    getPortalContainer?: () => HTMLElement;
    preventOverflow?: boolean;
    popperStyles?: CSSProperties;
    wrapperStyles?: CSSProperties;
    viewportGap?: number;
    handleCloseDuringScrolling?: () => void;
}
export interface ContentWrapperProps {
    centered: boolean;
}
export interface WrapperProps {
    width?: number;
}
export {};

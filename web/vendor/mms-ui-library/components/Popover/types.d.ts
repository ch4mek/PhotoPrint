import { Middleware, Placement } from '@floating-ui/react';
import { CSSProperties, MutableRefObject, ReactNode, Ref } from 'react';
import { CSSTransitionProps } from 'react-transition-group/CSSTransition';
export type RefElement = HTMLElement | null;
export type PopoverOffset = [number, number];
export interface PopoverProps {
    open: boolean;
    anchorElement: RefElement;
    id?: string;
    useAnchorWidth?: boolean;
    position?: Placement;
    preventFlip?: boolean;
    preventOverflow?: boolean;
    availableHeight?: boolean;
    setAvailableHeight?: (availableHeight: number) => void;
    withArrow?: boolean;
    arrowSize?: number;
    offset?: PopoverOffset;
    distanceToAnchorElement?: number;
    popperStyles?: CSSProperties;
    wrapperStyles?: CSSProperties;
    contentWrapperStyles?: CSSProperties;
    arrowStyles?: CSSProperties;
    getPortalContainer?: () => HTMLElement;
    transition?: CSSTransitionProps;
    transitionDuration?: CSSProperties['transitionDuration'];
    withTransition?: boolean;
    dataTestId?: string;
    update?: MutableRefObject<() => void>;
    className?: string;
    zIndex?: number;
    fallbackPlacements?: Placement[];
    children?: ReactNode;
    handleCloseDuringScrolling?: () => void;
    customMiddlewares?: Middleware[];
    ref?: Ref<HTMLDivElement>;
}
export interface ContentScrollableWrapperProps {
    availableHeight: boolean;
}
export interface WrapperProps {
    arrowSize: number;
}
export interface ArrowProps {
    arrowStyles: CSSProperties;
    arrowSize: number;
}

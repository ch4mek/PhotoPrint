import { HTMLAttributes, ReactElement, Ref, RefAttributes } from 'react';
import type { HintProps } from '../Hint';
export interface TooltipProps extends Omit<HintProps, 'open' | 'anchorElement' | 'ref'>, RefAttributes<HTMLElement> {
    children: ReactElement<{
        ref?: Ref<HTMLElement>;
    } & HTMLAttributes<HTMLElement>>;
    open?: boolean;
    showDelay?: number;
    useIfTextCut?: boolean;
    zIndex?: number;
    closeOnTouchStart?: boolean;
    closeOnAnchorTouchEnd?: boolean;
    scrollOffsetSubstraction?: number;
    textCutOrientation?: TextCutOrientation;
}
export type TextCutOrientation = 'vertical' | 'horizontal';

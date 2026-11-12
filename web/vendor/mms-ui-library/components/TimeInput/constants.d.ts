import { Placement } from '@floating-ui/react';
import { IMask } from 'react-imask';
import { InputSizes } from './types';
export declare const inputSizes: InputSizes;
export declare const INPUT_TOOLTIP_FALLBACK_PLACEMENTS: Array<Placement>;
export declare const textPositions: {
    s: number;
    m: number;
    l: number;
    half: string;
    max: string;
};
export declare const steps: {
    ArrowUp: number;
    ArrowDown: number;
};
export declare const MAX_HOURS = 23;
export declare const MAX_MINUTES = 59;
export declare const maskBlocks: {
    HH: {
        mask: typeof IMask.MaskedRange;
        from: number;
        to: number;
        maxLength: number;
        autofix: string;
    };
    MM: {
        mask: typeof IMask.MaskedRange;
        from: number;
        to: number;
        maxLength: number;
    };
};

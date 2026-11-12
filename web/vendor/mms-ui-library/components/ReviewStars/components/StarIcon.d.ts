import { SVGProps } from 'react';
import type { StarState } from '../types';
type StarIconProps = SVGProps<SVGSVGElement> & {
    state: StarState;
};
export declare function StarIcon({ state, ...props }: StarIconProps): import("react/jsx-runtime").JSX.Element;
export {};

import { RefAttributes, type SyntheticEvent } from 'react';
interface NavProps extends RefAttributes<HTMLSpanElement> {
    onClick: (e: SyntheticEvent<HTMLElement>) => void;
    open: boolean;
}
export declare function NavigationBurger({ onClick, open, ref }: NavProps): import("react/jsx-runtime").JSX.Element;
export {};

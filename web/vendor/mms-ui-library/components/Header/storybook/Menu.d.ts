import { SyntheticEvent } from 'react';
interface Props {
    showPopUp: (e: SyntheticEvent<HTMLElement>) => void;
    show: boolean;
}
export declare function Menu({ show, showPopUp }: Props): import("react/jsx-runtime").JSX.Element;
export {};

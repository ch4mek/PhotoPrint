import { MouseEvent as ReactMouseEvent } from 'react';
export declare const useHintState: () => {
    isHintOpen: boolean;
    setIsHintOpen: import("react").Dispatch<import("react").SetStateAction<boolean>>;
    indicatorRef: import("react").RefObject<HTMLElement | null>;
    toggleHintOpen: (event: ReactMouseEvent<HTMLElement>) => void;
    handleIndicatorRef: (node: HTMLElement | null) => void;
};

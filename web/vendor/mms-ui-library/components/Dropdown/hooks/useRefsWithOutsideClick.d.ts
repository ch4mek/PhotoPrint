export declare const useRefsWithOutsideClick: <T extends HTMLElement>(onOutside: () => void) => {
    mainRef: import("react").RefObject<T | null>;
    popoverRef: import("react").RefObject<T | null>;
};

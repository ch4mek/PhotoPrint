import { RefObject } from 'react';
export declare const useOnClickOutside: <T extends HTMLElement | null>(refs: RefObject<T>[], action: (e?: Event) => void) => void;

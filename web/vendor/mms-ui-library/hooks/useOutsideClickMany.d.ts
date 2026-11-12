import { RefObject } from 'react';
export declare const useOutsideClickMany: <T extends HTMLElement | null>(refs: RefObject<T>[] | RefObject<T>[], action: VoidFunction, portalId?: string, excludeId?: string) => void;

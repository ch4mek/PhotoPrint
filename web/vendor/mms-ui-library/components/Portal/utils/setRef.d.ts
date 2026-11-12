import { RefObject } from 'react';
export declare function setRef<T>(ref: RefObject<T> | ((instance: T | null) => void) | null | undefined, value: T | null): void;

import { MutableRefObject } from 'react';

export const isRefObject = <T>(
	ref: ((instance: T | null) => void) | MutableRefObject<T | null>
): ref is MutableRefObject<T | null> =>
	typeof ref === 'object' && ref !== null && 'current' in ref;

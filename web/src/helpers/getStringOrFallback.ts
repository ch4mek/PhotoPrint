import { isValueEmptyString } from './isValueEmptyString';

export const getStringOrFallback = <T = null>(
	value: string | null,
	fallback: T = null as T
): string | T => {
	if (!value) {
		return fallback;
	}

	return isValueEmptyString(value) ? fallback : value;
};

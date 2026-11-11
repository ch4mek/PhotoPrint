import isFunction from 'lodash/isFunction';
import { SetStateAction } from 'react';

export const handleSetStateAction = <T>(
	prevValue: T,
	payload: SetStateAction<T>
): T => {
	if (isFunction(payload)) {
		return payload(prevValue);
	}

	return payload;
};

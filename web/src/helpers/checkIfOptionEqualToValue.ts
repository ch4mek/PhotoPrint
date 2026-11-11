import { WithId } from '@/types/helpers';

import {
	areObjectsFieldEqual,
	arePrimitiveValuesEqual,
	Comparator,
} from './areValuesEqual';

type FieldComparator<T, K extends keyof T> = (a: T, b: T, key: K) => boolean;

function checkIfOptionEqualToValue<T, K extends keyof T>(
	comparator: FieldComparator<T, K>,
	key: K
): (option1: T, option2: T) => boolean;

function checkIfOptionEqualToValue<ValueType>(
	comparator: Comparator<ValueType>
): (option1: ValueType, option2: ValueType) => boolean;

function checkIfOptionEqualToValue<T, K extends keyof T>(
	comparator: FieldComparator<T, K> | Comparator<T>,
	key?: K
) {
	if (key === undefined) {
		return (option1: T, option2: T): boolean =>
			(comparator as Comparator<T>)(option1, option2);
	}

	return (option1: T, option2: T): boolean =>
		(comparator as FieldComparator<T, K>)(option1, option2, key);
}

export const checkIfOptionsEqualById = checkIfOptionEqualToValue<WithId, 'id'>(
	areObjectsFieldEqual,
	'id'
);

export const checkIfPrimitiveOptionsEqual = checkIfOptionEqualToValue(
	arePrimitiveValuesEqual
);

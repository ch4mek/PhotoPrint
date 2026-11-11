import { WithId } from '@/types/helpers';

export const extractKeyFromPrimitive = <T>(value: T): T => value;
const extractKeyWithKeyField =
	<T extends object, K extends keyof T>(keyField: K) =>
	(option: T): T[K] =>
		option[keyField];

export const extractKeyById = extractKeyWithKeyField<WithId, 'id'>('id');

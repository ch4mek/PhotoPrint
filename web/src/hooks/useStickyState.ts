import { useEffect, useState } from 'react';

import { useAuth } from '../context';

import { DataTypes } from '@/constants/dataTypes';

export function useStickyState<T = unknown>(
	defaultValue: T,
	key: string,
	dataType?: DataTypes,
	storage = sessionStorage
) {
	const { id } = useAuth();
	const stickyKey = `${key}/${id}`;

	const [value, setValue] = useState<T>(() => {
		if (!storage) {
			return defaultValue;
		}

		const stickyValue = storage.getItem(stickyKey);

		if (dataType === DataTypes.Date) {
			return stickyValue !== null
				? new Date(JSON.parse(stickyValue))
				: defaultValue;
		}

		return stickyValue !== null ? JSON.parse(stickyValue) : defaultValue;
	});

	useEffect(() => {
		if (dataType === DataTypes.Date) {
			storage.setItem(stickyKey, JSON.stringify(Number(value)));
		} else {
			storage.setItem(stickyKey, JSON.stringify(value));
		}
	}, [key, value]);

	return [value, setValue] as const;
}

import { useMemo, useReducer } from 'react';

import { defaultUserEquipmentFilters } from '../constants';
import { UserEquipmentFiltersReducer } from '../types';
import {
	createUserEquipmentFiltersHandlers,
	userEquipmentReducer,
} from '../utils';

export function useEquipmentFiltersReducer(): UserEquipmentFiltersReducer {
	const [filters, dispatch] = useReducer(
		userEquipmentReducer,
		defaultUserEquipmentFilters
	);

	const setFilters = useMemo(
		() => createUserEquipmentFiltersHandlers(filters, dispatch),
		[filters]
	);

	return [filters, setFilters];
}

import { useCallback, useEffect, useMemo, useReducer } from 'react';

import { DEFAULT_CONVERSATION_FILTER_VALUES } from '../constants';

import { FromToProps } from '@/components/Grid';
import { SortDirection } from '@/constants/sort';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { useTableState } from '@/pages/hooks/useTableState';
import {
	conversationFiltersReducer,
	generateSetFilters,
} from '@/pages/TeamBoardUser/components/ConversationsTab/utils';

export const useConversationFilters = (
	initialFilters = DEFAULT_CONVERSATION_FILTER_VALUES
) => {
	const [filters, dispatch] = useReducer(
		conversationFiltersReducer,
		DEFAULT_CONVERSATION_FILTER_VALUES
	);
	const {
		order,
		orderBy,
		handleSortColumnChange,
		sortColumns,
		page,
		setPage,
		rowsPerPage,
		handleOnPerPageChange,
	} = useTableState({
		initialOrder: SortDirection.DESC,
		initialOrderBy: 'date',
	});

	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	const areFiltersApplied = useMemo(
		() =>
			(Object.keys(filters) as Array<keyof typeof filters>).some((key) => {
				if (Array.isArray(filters[key])) {
					return !areArraysEqualByPrimitiveItems(
						filters[key] as string[],
						initialFilters[key] as string[]
					);
				}

				if (typeof filters[key] === 'object') {
					return !areFromToCompatibleObjectsEqual(
						filters[key] as FromToProps<Date>,
						initialFilters[key] as FromToProps<Date>
					);
				}

				return !arePrimitiveValuesEqual(
					filters[key] as string,
					initialFilters[key] as string
				);
			}),
		[filters, initialFilters]
	);

	useEffect(() => {
		setFilters.set(initialFilters);
	}, [initialFilters]);

	const resetFilters = useCallback(() => {
		setFilters.set(initialFilters);
	}, [initialFilters]);

	return {
		page,
		setPage,
		filters,
		setFilters,
		areFiltersApplied,
		rowsPerPage,
		resetFilters,
		order,
		orderBy,
		handleSortColumnChange,
		sortColumns,
		handleOnPerPageChange,
	};
};

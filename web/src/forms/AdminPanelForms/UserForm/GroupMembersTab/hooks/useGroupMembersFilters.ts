import { useMemo, useReducer } from 'react';

import { DEFAULT_GROUP_MEMBERS_FILTER_VALUES } from '../constants';
import { generateSetFilters } from '../utils/generateSetFilters';
import { groupMembersFiltersReducer } from '../utils/groupMembersFiltersReducer';

export const useGroupMembersFilters = () => {
	const [filters, dispatch] = useReducer(
		groupMembersFiltersReducer,
		DEFAULT_GROUP_MEMBERS_FILTER_VALUES,
		() => DEFAULT_GROUP_MEMBERS_FILTER_VALUES
	);

	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	return { filters, setFilters };
};

import {
	areFromToCompatibleObjectsEqual,
	areArraysEqualByPrimitiveItems,
} from '@/helpers/areValuesEqual';
import {
	TRACKED_TIME_DEFAULT_VALUE,
	IS_ACTIVE_DEFAULT_VALUE,
} from '@/pages/constants';

import { PROJECT_FILTERS_INITIAL_STATE } from './constants';
import { ProjectsFilters } from './types';

export const compareProjectsFilters = ({
	projectFilter,
	clientFilter,
	idFilter,
	publicFilter,
	statusFilter,
	billableFilter,
	billableTrackedTimeFilter,
	factTrackedTimeFilter,
}: ProjectsFilters) => ({
	projectFilter: projectFilter === PROJECT_FILTERS_INITIAL_STATE.projectFilter,
	clientFilter: clientFilter === PROJECT_FILTERS_INITIAL_STATE.clientFilter,
	idFilter: idFilter === PROJECT_FILTERS_INITIAL_STATE.idFilter,
	factTrackedTimeFilter: areFromToCompatibleObjectsEqual(
		factTrackedTimeFilter,
		TRACKED_TIME_DEFAULT_VALUE
	),
	billableTrackedTimeFilter: areFromToCompatibleObjectsEqual(
		billableTrackedTimeFilter,
		TRACKED_TIME_DEFAULT_VALUE
	),
	statusFilter: areArraysEqualByPrimitiveItems(
		statusFilter,
		IS_ACTIVE_DEFAULT_VALUE
	),
	publicFilter: publicFilter.length === 0,
	billableFilter: billableFilter.length === 0,
});

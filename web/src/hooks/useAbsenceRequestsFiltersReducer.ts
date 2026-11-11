import { useMemo, useReducer } from 'react';

import {
	absenceRequestsFiltersReducer,
	createAbsenceRequestFilters,
} from '@/components/UserProfile/components/AbsencesTab/components/AbsenceRequests/reducer';
import { defaultAbsenceRequestFilterValues } from '@/constants/absence';
import { AbsenceRequestsFilterReducer } from '@/types/absences';

export function useAbsenceRequestsFiltersReducer(): AbsenceRequestsFilterReducer {
	const [filters, dispatch] = useReducer(
		absenceRequestsFiltersReducer,
		defaultAbsenceRequestFilterValues
	);

	const setFilters = useMemo(
		() => createAbsenceRequestFilters(filters, dispatch),
		[filters]
	);

	return [filters, setFilters];
}

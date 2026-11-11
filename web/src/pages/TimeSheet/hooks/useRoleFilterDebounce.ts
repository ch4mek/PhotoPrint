import { useEffect, useMemo, useRef } from 'react';

import { CLOSE_DELAY_ROLE_FILTER_MS, timesheetModesIds } from '../constants';
import { TimesheetModeFilterOption } from '../types';

import { useDebounce } from '@/hooks/useDebounce';

const { personalModeId } = timesheetModesIds;

export const useDepartmentTypeFilterDebounce = (
	departmentTypeFilter: TimesheetModeFilterOption
) => {
	const prevDepartmentTypeFilterChange =
		useRef<TimesheetModeFilterOption | null>(null);
	const debouncedValue = useDebounce(
		departmentTypeFilter,
		CLOSE_DELAY_ROLE_FILTER_MS
	);

	useEffect(() => {
		prevDepartmentTypeFilterChange.current = departmentTypeFilter;
	}, [departmentTypeFilter]);

	return useMemo(
		() =>
			departmentTypeFilter.fullName === personalModeId ||
			(prevDepartmentTypeFilterChange.current &&
				prevDepartmentTypeFilterChange.current.fullName === personalModeId)
				? debouncedValue
				: departmentTypeFilter,
		[
			departmentTypeFilter,
			prevDepartmentTypeFilterChange.current,
			debouncedValue,
		]
	);
};

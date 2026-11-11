import { useTheme } from '@emotion/react';
import { useCallback, useMemo } from 'react';

import {
	defaultTimesheetModes,
	timesheetModesForGroupManager,
	timesheetModesForHeadProjectManager,
	timesheetModesForProjectManager,
} from '../../constants';
import { useLocalizedTimesheetModeOptions } from '../../hooks/useLocalizedTimesheetModeOptions';
import { TimesheetModeFilterOption } from '../../types';
import { getInitialTimesheetsModeFilters } from '../../utils/getInitialTimesheetsModeFilter';
import { resolveCanonicalTimesheetModeChange } from '../../utils/localizeTimesheetModeOption';
import { setFilterByIdIfChanged } from '../TimesheetPageControls/utils';

import { FilterAutoselect } from '@/components/FilterAutoselect';
import { isPMO } from '@/constants/departments';
import { isGroupManager, isHeadOfDepartment } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';

import type { UserDropdownProps } from './types';

const getKey = (option: TimesheetModeFilterOption) => option.id;

export function UserDropdown({
	departmentTypeFilter,
	setDepartmentTypeFilter,
	isMobileView = false,
}: UserDropdownProps) {
	const theme = useTheme();
	const { departmentId, role } = useAuth();
	const { localizeOption, localizeOptions } =
		useLocalizedTimesheetModeOptions();
	const stylesConfig = {
		backgroundColor: theme.palette['background-white'],
	};

	const initialValue = useMemo(
		() => getInitialTimesheetsModeFilters(role, departmentId),
		[role, departmentId]
	);

	const handleSearchChange = () => {};

	const options = useMemo(() => {
		if (isGroupManager(role)) {
			return timesheetModesForGroupManager;
		}

		if (!isPMO(departmentId)) {
			return defaultTimesheetModes;
		}

		if (isHeadOfDepartment(role)) {
			return timesheetModesForHeadProjectManager;
		}

		return timesheetModesForProjectManager;
	}, [departmentId, role]);

	const localizedOptions = useMemo(
		() => localizeOptions(options),
		[localizeOptions, options]
	);

	const localizedInitialValue = useMemo(
		() => localizeOption(initialValue),
		[initialValue, localizeOption]
	);

	const displayValue = useMemo(() => {
		const current = departmentTypeFilter?.fullName
			? departmentTypeFilter
			: initialValue;
		const canonical =
			options.find((option) => option.id === current.id) ?? initialValue;

		return localizeOption(canonical);
	}, [departmentTypeFilter, initialValue, localizeOption, options]);

	const handleDepartmentTypeFilterChange = useCallback(
		(newRole: TimesheetModeFilterOption | null) => {
			const nextValue = resolveCanonicalTimesheetModeChange(
				newRole,
				options,
				initialValue
			);

			setFilterByIdIfChanged(setDepartmentTypeFilter, nextValue);
		},
		[initialValue, options, setDepartmentTypeFilter]
	);

	return (
		<FilterAutoselect
			label=""
			getKey={getKey}
			value={displayValue}
			options={localizedOptions}
			valueField="fullName"
			valueToClean={localizedInitialValue}
			onChange={handleDepartmentTypeFilterChange}
			stylesConfig={stylesConfig}
			preventDropdownFlip
			checkIfOptionEqualToValue={checkIfOptionsEqualById}
			onSearchInputChange={handleSearchChange}
			isMobileView={isMobileView}
		/>
	);
}

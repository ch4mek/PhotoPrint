import { useTheme } from '@emotion/react';
import { useCallback, useMemo } from 'react';

import { defaultInitialTimesheetMode } from '../../constants';
import { useLocalizedTimesheetModeOptions } from '../../hooks/useLocalizedTimesheetModeOptions';
import { TimesheetModeFilterOption } from '../../types';
import { resolveCanonicalTimesheetModeChange } from '../../utils/localizeTimesheetModeOption';
import { setFilterByIdIfChanged } from '../TimesheetPageControls/utils';

import { FilterAutoselect } from '@/components/FilterAutoselect';
import { useConfiguration } from '@/context/Configuration';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { expandDepartmentsData } from '@/helpers/expandDepartmentsData';
import { extractKeyById } from '@/helpers/extractKey';

import type { RoleSelectWithDepartmentsProps } from './types';

export function RoleDropdownWithDepartments({
	departmentTypeFilter,
	setDepartmentTypeFilter,
	isMobileView = false,
}: RoleSelectWithDepartmentsProps) {
	const { localizeOption, localizeOptions } =
		useLocalizedTimesheetModeOptions();
	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();
	const dataExpanded = useMemo(
		() => expandDepartmentsData(departmentTypes) ?? [],
		[departmentTypes]
	);

	const localizedOptions = useMemo(
		() => localizeOptions(dataExpanded),
		[dataExpanded, localizeOptions]
	);

	const localizedInitialValue = useMemo(
		() => localizeOption(defaultInitialTimesheetMode),
		[localizeOption]
	);

	const displayValue = useMemo(() => {
		const current = departmentTypeFilter?.fullName
			? departmentTypeFilter
			: defaultInitialTimesheetMode;
		const canonical =
			dataExpanded.find((option) => option.id === current.id) ?? current;

		return localizeOption(canonical);
	}, [departmentTypeFilter, dataExpanded, localizeOption]);

	const handleSearchChange = () => {};

	const handleDepartmentTypeFilterChange = useCallback(
		(newRole: TimesheetModeFilterOption | null) => {
			const nextValue = resolveCanonicalTimesheetModeChange(
				newRole,
				dataExpanded,
				defaultInitialTimesheetMode
			);

			setFilterByIdIfChanged(setDepartmentTypeFilter, nextValue);
		},
		[dataExpanded, setDepartmentTypeFilter]
	);

	const theme = useTheme();

	const stylesConfig = useMemo(
		() => ({
			backgroundColor: theme.palette['background-white'],
		}),
		[theme]
	);

	return (
		<FilterAutoselect
			value={displayValue}
			options={localizedOptions}
			onChange={handleDepartmentTypeFilterChange}
			stylesConfig={stylesConfig}
			label=""
			getKey={extractKeyById}
			valueToClean={localizedInitialValue}
			valueField="fullName"
			checkIfOptionEqualToValue={checkIfOptionsEqualById}
			onSearchInputChange={handleSearchChange}
			isMobileView={isMobileView}
		/>
	);
}

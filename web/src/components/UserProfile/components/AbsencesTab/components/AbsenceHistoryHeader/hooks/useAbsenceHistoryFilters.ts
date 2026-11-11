import { useCallback, useMemo } from 'react';

import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import {
	initialAbsenceFilterValue,
	initialYearFilterValue,
} from '@/constants/absence';
import { useAbsenceHistoryContext } from '@/context/Absence';
import { convertConstantToSelectionModalOption } from '@/helpers/convertToSelectionModalOptions';
import { generateYearsCompanyExistence } from '@/helpers/date';
import { UserAbsenceItem } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export const useAbsenceHistoryFilters = (absences: UserAbsenceItem[]) => {
	const { setYearFilter, setAbsenceFilter, yearFilter, absenceFilter } =
		useAbsenceHistoryContext();

	const yearOptions = useMemo(
		() => [
			initialYearFilterValue,
			...generateYearsCompanyExistence().map((value) =>
				convertConstantToSelectionModalOption(value.toString())
			),
		],
		[]
	);

	const absencesOptions = useMemo(
		() =>
			absences.map(({ name }) => ({
				id: name,
				name,
			})),
		[absences]
	);

	const handleAbsenceFilterChange = useCallback(
		(value: SimpleConfigurableConfigurationItem | null) => {
			if (value) {
				setAbsenceFilter(value);
			}
		},
		[]
	);

	const handleYearChange = useCallback((value: SelectionModalOption | null) => {
		if (value) {
			setYearFilter(value);
		}
	}, []);

	return {
		yearFilter,
		absenceFilter,
		handleAbsenceFilterChange,
		handleYearChange,
		yearOptions,
		absenceTypesOptions: [initialAbsenceFilterValue, ...absencesOptions],
	};
};

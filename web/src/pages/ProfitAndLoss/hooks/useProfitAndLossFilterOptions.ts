import { useMemo } from 'react';

import { ProfitAndLossFilterOptions } from '../types';

import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { useConfiguration } from '@/context/Configuration';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { getFullName } from '@/helpers/utils';
import { useRatesUsersLookup } from '@/queries/rates/useRatesUsersLookup';

export const useProfitAndLossFilterOptions = (year: number, month: number) => {
	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const departments = useMemo(
		() => [...departmentTypes, NO_VALUE_FILTER_OPTION],
		[departmentTypes]
	);

	const { data: users } = useRatesUsersLookup(year, month);
	const userOptions = useMemo(
		() =>
			(users || []).map(({ firstName, lastName, firstNameRu, lastNameRu }) =>
				getFullName(firstName, lastName, firstNameRu, lastNameRu)
			),
		[users]
	);

	const options = useMemo(
		(): ProfitAndLossFilterOptions => ({
			users: userOptions,
			departments: convertToSelectionModalOptions(departments, 'fullName'),
		}),
		[userOptions, departments]
	);

	return { options, allDepartmentsLength: departments.length };
};

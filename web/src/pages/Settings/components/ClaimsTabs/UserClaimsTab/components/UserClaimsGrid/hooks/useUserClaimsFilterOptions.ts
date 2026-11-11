import { useMemo } from 'react';

import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { AllRoles } from '@/constants/roles';
import { useConfiguration } from '@/context/Configuration';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { getFullName } from '@/helpers/formatName';
import { useUsersLookup } from '@/queries/useUsersLookup';

export function useUserClaimsFilterOptions() {
	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();
	const { data: users } = useUsersLookup({
		isActive: true,
		roles: AllRoles,
	});

	const options = useMemo(() => {
		const userOptions = users
			? users.map(({ firstName, lastName }) => getFullName(firstName, lastName))
			: [];

		const departments = [
			...convertToSelectionModalOptions(departmentTypes, 'fullName'),
			NO_VALUE_FILTER_OPTION,
		];

		return { users: userOptions, departments };
	}, [users, departmentTypes]);

	return { options, departmentTypes };
}

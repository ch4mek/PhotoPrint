import { useMemo } from 'react';

import { Roles } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { UsersLookupFiltersParams } from '@/queries/types';
import { useUsersLookupFilters } from '@/queries/useUsersLookupFilters';

export const useUserFilter = (
	requestParams: Partial<UsersLookupFiltersParams>,
	acceptedRoles: Roles[],
	isReports = true,
	onlyManagedUsers?: boolean
) => {
	const { role, departmentId } = useAuth();

	const toast = useToast();

	const { data: users, isFetching: isUserFilterFetching } =
		useUsersLookupFilters(
			requestParams,
			toast,
			role,
			isReports,
			onlyManagedUsers,
			departmentId,
			{
				onError: () => createOnErrorHandler(toast),
			}
		);

	return useMemo(
		() =>
			!acceptedRoles.includes(role) || isUserFilterFetching || !users?.length
				? []
				: users.map(({ id, lastName, firstName }) => ({
						id,
						name: `${lastName} ${firstName}`,
				  })),
		[isUserFilterFetching]
	);
};

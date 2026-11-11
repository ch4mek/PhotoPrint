import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { DateFormat } from '@/constants/date';
import { isPMO } from '@/constants/departments';
import { dashboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USERS_API_ENDPOINT } from '@/constants/queryPaths';
import { isManagerRoleOrGroupManager, Roles } from '@/constants/roles';
import { ToastContextValues } from '@/context/types';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import dayjs from '@/lib/dayjs';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import type { LookupFilterUserResponse } from '@/types/userInfo';

import { ServerResponseError, UsersLookupFiltersParams } from './types';

export const useUsersLookupFilters = (
	{
		keyWord,
		clientId,
		projectId,
		departmentId,
		period,
		resourceGroupId,
	}: Partial<UsersLookupFiltersParams>,
	toast: ToastContextValues,
	role: Roles,
	isReports = false,
	onlyManagedUsers?: boolean,
	userDepartmentId?: string,
	options?: PatchedQueryOptions<
		LookupFilterUserResponse[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback({
		queryKey: [
			dashboardQueriesKeys.getTimesheetUsersLookup,
			keyWord,
			clientId,
			projectId,
			departmentId,
			userDepartmentId,
			onlyManagedUsers,
			period,
			resourceGroupId,
		],
		queryFn: () =>
			axios
				.get<Array<LookupFilterUserResponse>>(
					`${GET_USERS_API_ENDPOINT}/lookup/filters${
						isReports ? '/reports' : ''
					}`,
					{
						params: {
							keyWord,
							clientId,
							projectId,
							departmentId,
							resourceGroupId,
							onlyManagedUsers: onlyManagedUsers ? true : undefined,
							reportForDateFrom: dayjs(period?.[0]).format(
								DateFormat.YYYY_MM_DD
							),
							reportForDateTo: dayjs(period?.at(-1)).format(
								DateFormat.YYYY_MM_DD
							),
						},
					}
				)
				.then((res) => res.data)
				.catch(handleRequestError),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		enabled: isManagerRoleOrGroupManager(role) || isPMO(userDepartmentId),
		...options,
	});

import axios, { AxiosError } from 'axios';

import { dashboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_EMPLOYEE_EVENTS_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { EmployeeEvent } from '@/types/dashboard';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetEmployeeEvents = (
	departmentId: string | null,
	options?: PatchedQueryOptions<
		EmployeeEvent[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback({
		queryKey: [dashboardQueriesKeys.getEmployeeEvents, departmentId],
		queryFn: () =>
			axios
				.get<EmployeeEvent[]>(GET_EMPLOYEE_EVENTS_API_ENDPOINT, {
					params: {
						departmentId: departmentId || undefined,
					},
				})
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});

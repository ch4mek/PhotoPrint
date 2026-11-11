import axios, { AxiosError } from 'axios';

import { dashboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_PENDING_INQUIRIES } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { RequestsWidgetResponse } from '@/types/dashboard';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetRequestsWidget = (
	departmentId: string | null,
	options?: PatchedQueryOptions<
		RequestsWidgetResponse[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		RequestsWidgetResponse[],
		AxiosError<ServerResponseError>
	>({
		queryKey: [dashboardQueriesKeys.getRequestsWidget, departmentId],
		queryFn: async () => {
			try {
				const res = await axios.get<RequestsWidgetResponse[]>(
					GET_PENDING_INQUIRIES,
					{
						params: {
							departmentId: departmentId || undefined,
						},
					}
				);

				return res.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		refetchOnWindowFocus: false,
		refetchOnMount: true,
		...options,
	});

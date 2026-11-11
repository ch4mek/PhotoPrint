import axios, { AxiosError } from 'axios';

import { homeQueriesKeys } from '@/constants/queriesKeys';
import { getUserUpcomingEvents } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { UpcomingEvent } from '@/types/home';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

const OVERDUE_DAYS_COUNT = 7;

export const useGetUserUpcomingEvents = (
	userId?: string | number,
	options?: PatchedQueryOptions<
		UpcomingEvent[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback({
		queryKey: [homeQueriesKeys.getUserUpcomingEvents, userId],
		queryFn: () =>
			axios
				.get<UpcomingEvent[]>(getUserUpcomingEvents(userId!), {
					params: {
						overdueDaysCount: OVERDUE_DAYS_COUNT,
					},
				})
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});

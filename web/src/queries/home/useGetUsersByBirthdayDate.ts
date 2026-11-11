import axios, { AxiosError } from 'axios';

import { homeQueriesKeys } from '@/constants/queriesKeys';
import { getUsersByBirthdayDate } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { UserBirthday } from '@/types/home';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetUsersByBirthdayDate = (
	date: string,
	options?: PatchedQueryOptions<UserBirthday[], AxiosError<ServerResponseError>>
) =>
	useQueryWithCallback({
		queryKey: [homeQueriesKeys.getBirthdayEmployeesByDate, date],
		queryFn: () =>
			axios
				.get<UserBirthday[]>(getUsersByBirthdayDate(date), {})
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});

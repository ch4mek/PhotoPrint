import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { interviewQueriesKeys } from '@/constants/queriesKeys';
import { getInterviewUsersLookupEndpoint } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { LookupUserResponse } from '@/types/userInfo';

export const useInterviewUsersLookup = (
	onlyActive?: boolean,
	queryOptions?: PatchedQueryOptions<Array<LookupUserResponse>, AxiosError>
) =>
	useQueryWithCallback<Array<LookupUserResponse>, AxiosError>({
		queryKey: [interviewQueriesKeys.getInterviewUsersLookup, onlyActive],
		queryFn: () =>
			axios
				.get<Array<LookupUserResponse>>(
					getInterviewUsersLookupEndpoint(onlyActive)
				)
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...queryOptions,
	});

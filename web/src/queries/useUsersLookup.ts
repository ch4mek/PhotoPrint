import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { UsersQueriesKeys } from '@/constants/queriesKeys';
import { GET_USERS_API_ENDPOINT } from '@/constants/queryPaths';
import { getKeywordQuery } from '@/helpers/queries';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { LookupUserResponse } from '@/types/userInfo';

interface UseUsersLookupOptions {
	search?: string;
	roles?: Array<string>;
	queryOptions?: PatchedQueryOptions<Array<LookupUserResponse>, AxiosError>;
	isActive?: boolean;
}

export const useUsersLookup = ({
	search = '',
	roles = [],
	queryOptions,
	isActive,
}: UseUsersLookupOptions = {}) => {
	const query = getKeywordQuery({ search, roles, isActive });

	return useQueryWithCallback<Array<LookupUserResponse>, AxiosError>({
		...queryOptions,
		queryKey: [UsersQueriesKeys.usersLookup, search],
		queryFn: () =>
			axios
				.get<Array<LookupUserResponse>>(
					`${GET_USERS_API_ENDPOINT}/lookup${query}`
				)
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
};

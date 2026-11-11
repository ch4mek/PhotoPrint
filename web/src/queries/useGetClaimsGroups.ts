import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GROUPS_CLAIMS_QUERY_KEY } from '@/constants/queriesKeys';
import { GET_CLAIM_GROUPS_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { UserClaimsGroup } from '@/pages/Settings/components/ClaimsTabs/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { ServerResponseError } from './types';

export const useGetClaimsGroups = (
	options?: PatchedQueryOptions<
		UserClaimsGroup[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<UserClaimsGroup[], AxiosError<ServerResponseError>>({
		...options,
		queryKey: [GROUPS_CLAIMS_QUERY_KEY],
		queryFn: () => axios.get(GET_CLAIM_GROUPS_ENDPOINT).then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});

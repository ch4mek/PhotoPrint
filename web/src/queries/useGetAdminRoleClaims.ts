import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { SettingsRoleClaimsQueriesKeys } from '@/constants/queriesKeys';
import { GET_SETTINGS_ROLE_CLAIMS_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { GetAdminRoleClaimsResponse, ServerResponseError } from './types';

export const useGetRoleClaims = (
	options?: PatchedQueryOptions<
		GetAdminRoleClaimsResponse[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		GetAdminRoleClaimsResponse[],
		AxiosError<ServerResponseError>
	>({
		...options,
		queryKey: [SettingsRoleClaimsQueriesKeys.roleClaims],
		queryFn: () =>
			axios
				.get(GET_SETTINGS_ROLE_CLAIMS_ENDPOINT)
				.then((res) => res.data)
				.catch(handleRequestError),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});

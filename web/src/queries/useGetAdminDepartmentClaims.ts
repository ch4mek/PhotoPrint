import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { SettingsDepartmentClaimsQueriesKeys } from '@/constants/queriesKeys';
import { GET_SETTINGS_DEPARTMENT_CLAIMS_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { GetAdminDepartmentClaimsResponse, ServerResponseError } from './types';

export const useGetDepartmentClaims = (
	options?: PatchedQueryOptions<
		GetAdminDepartmentClaimsResponse[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		GetAdminDepartmentClaimsResponse[],
		AxiosError<ServerResponseError>
	>({
		queryKey: [SettingsDepartmentClaimsQueriesKeys.departmentClaims],
		queryFn: () =>
			axios
				.get(GET_SETTINGS_DEPARTMENT_CLAIMS_ENDPOINT)
				.then((res) => res.data)
				.catch(handleRequestError),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...options,
	});

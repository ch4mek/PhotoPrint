import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { departmentQueryKeys } from '@/constants/queriesKeys';
import { getDepartmentGroupManagersEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import {
	GetDepartmentGroupManagersEndpointParams,
	GetDepartmentGroupManagersResponse,
} from './types';

export const useGetDepartmentGroupManagers = (
	departmentId: string,
	params: GetDepartmentGroupManagersEndpointParams,
	options?: PatchedQueryOptions<
		GetDepartmentGroupManagersResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		GetDepartmentGroupManagersResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			departmentQueryKeys.getDepartmentGroupManager,
			departmentId,
			params,
		],
		queryFn: async () => {
			try {
				const { data } = await axios.get<GetDepartmentGroupManagersResponse>(
					getDepartmentGroupManagersEndpoint(departmentId),
					{ params }
				);

				return data;
			} catch (err) {
				return handleRequestError(err);
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...options,
	});

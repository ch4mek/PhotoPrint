import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { departmentQueryKeys } from '@/constants/queriesKeys';
import { GET_DEPARTMENTS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { DepartmentResponse } from '@/types/department';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetDepartments = (
	options?: Partial<
		PatchedQueryOptions<DepartmentResponse[], AxiosError<ServerResponseError>>
	>
) =>
	useQueryWithCallback<DepartmentResponse[], AxiosError<ServerResponseError>>({
		queryKey: [departmentQueryKeys.getDepartments],
		queryFn: () =>
			axios.get(GET_DEPARTMENTS_API_ENDPOINT).then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...options,
	});

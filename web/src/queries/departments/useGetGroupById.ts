import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { departmentQueryKeys } from '@/constants/queriesKeys';
import { departmentGroupEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { Group } from '@/types/department';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetGroupById = (
	id?: number,
	options?: PatchedQueryOptions<Group, AxiosError<ServerResponseError>>
) =>
	useQueryWithCallback<Group, AxiosError<ServerResponseError>>({
		...options,
		queryKey: [departmentQueryKeys.getGroupById, id],
		queryFn: async () => {
			try {
				const { data } = await axios.get<Group>(departmentGroupEndpoint(id));

				return data;
			} catch (err) {
				return handleRequestError(err);
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});

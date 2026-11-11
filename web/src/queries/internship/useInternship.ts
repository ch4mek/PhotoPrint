import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { HttpStatuses } from '@/constants/httpStatuses';
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { getInternshipEndpoint } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { InternshipResponse } from '@/types/internship';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useInternship = (
	userId?: number,
	queryOptions?: PatchedQueryOptions<
		InternshipResponse | null,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		InternshipResponse | null,
		AxiosError<ServerResponseError>
	>({
		...queryOptions,
		queryKey: [TeamboardQueriesKeys.internship, userId],
		queryFn: async () => {
			const response = await axios.get<InternshipResponse>(
				getInternshipEndpoint(userId!)
			);

			if (response.status === HttpStatuses.NoContent || !response.data) {
				return null;
			}

			return response.data;
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		enabled: Boolean(userId),
	});

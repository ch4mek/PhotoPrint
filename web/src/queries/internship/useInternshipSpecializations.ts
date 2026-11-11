import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { LabboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_INTERNSHIP_SPECIALIZATIONS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { InternshipSpecializationLookupItem } from '@/types/internship';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useInternshipSpecializations = (
	queryOptions?: PatchedQueryOptions<
		InternshipSpecializationLookupItem[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		InternshipSpecializationLookupItem[],
		AxiosError<ServerResponseError>
	>({
		...queryOptions,
		queryKey: [LabboardQueriesKeys.internshipSpecializations],
		queryFn: async () => {
			const { data } = await axios.get<InternshipSpecializationLookupItem[]>(
				GET_INTERNSHIP_SPECIALIZATIONS_API_ENDPOINT
			);

			return data;
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
	});

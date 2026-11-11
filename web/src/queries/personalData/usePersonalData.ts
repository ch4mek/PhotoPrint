import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_PERSONAL_DATA_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { PersonalDataResponse } from '@/types/personalData';

export const usePersonalData = (
	id: string | number | undefined,
	queryOptions?: PatchedQueryOptions<
		PersonalDataResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<PersonalDataResponse, AxiosError<ServerResponseError>>({
		...queryOptions,
		queryKey: [TeamboardQueriesKeys.personalData, id],
		queryFn: async () => {
			const { data } = await axios.get<PersonalDataResponse>(
				`${GET_PERSONAL_DATA_API_ENDPOINT}`,
				{
					params: {
						userId: id,
					},
				}
			);

			return data;
		},

		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		enabled: Boolean(id),
	});

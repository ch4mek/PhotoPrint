import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ByPageResponse } from '../types';
import { convertFromToDateObjToQueryParam } from '../utils';

import { PayslipsQueriesKeys } from '@/constants/queriesKeys';
import { getUserPayslipsEndpoint } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { UserPayslips } from '@/types/teamboard';

import { GetPayslipsQueryParams } from './types';

export const useUserPayslips = (params: GetPayslipsQueryParams) => {
	const { top, skip, filters, userId, orderBy } = params;

	return useQueryWithCallback<ByPageResponse<UserPayslips>, AxiosError>({
		queryKey: [
			PayslipsQueriesKeys.getUserPayslips,
			`${userId}`,
			filters,
			orderBy,
			top,
			skip,
		],
		queryFn: async () => {
			const query = buildQuery({
				filter: {
					and: [
						...convertFromToDateObjToQueryParam(
							filters?.reportDate,
							'reportDate'
						),
					],
				},
				top,
				skip,
				orderBy,
			});

			const { data } = await axios.get<ByPageResponse<UserPayslips>>(
				`${getUserPayslipsEndpoint(userId!)}${query}`
			);

			return data;
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		enabled: Boolean(userId),
	});
};

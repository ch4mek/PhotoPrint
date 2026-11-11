import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import numeral from 'numeral';
import buildQuery from 'odata-query';

import { ByPageResponse } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { getUserSalaryEndpoint } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { SalaryTableFilters } from '@/pages/TeamBoardUser/components/SalaryTable/types';
import { convertFromToDateObjToQueryParam } from '@/queries/utils';
import { TeamboardTableMode, UserSalary } from '@/types/teamboard';

import { GetSalaryQueryParams } from './types';

const { PopUp } = TeamboardTableMode;

export const useUserSalaries = (params: GetSalaryQueryParams) => {
	const { top, skip, filters, userId, orderBy, mode, conversationId } = params;
	const { date, salaryUsd } = filters || ({} as SalaryTableFilters);

	if (mode === TeamboardTableMode.PopUp && !conversationId) {
		return {
			data: null,
			refetch: () => {},
			isFetching: false,
			isLoading: false,
		};
	}

	return useQueryWithCallback<ByPageResponse<UserSalary>, AxiosError>({
		queryKey: [
			TeamboardQueriesKeys.getUserSalaries,
			`${userId}`,
			`${conversationId}`,
			filters,
			orderBy,
			top,
			skip,
		],
		queryFn: async () => {
			const query = buildQuery({
				filter: {
					and: [
						conversationId && `conversationId eq ${conversationId}`,
						...convertFromToDateObjToQueryParam(date, 'salaryFrom'),
						salaryUsd?.from &&
							`salaryUsd ge ${numeral(salaryUsd.from).value()}`,
						salaryUsd?.to && `salaryUsd le ${numeral(salaryUsd.to).value()}`,
						mode && mode === PopUp && `source eq '${PopUp}'`,
					],
				},
				top,
				skip,
				orderBy,
			});

			const { data } = await axios.get<ByPageResponse<UserSalary>>(
				`${getUserSalaryEndpoint(userId!)}${query}`
			);

			return data;
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		enabled: Boolean(userId),
	});
};

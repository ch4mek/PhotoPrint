import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ratesQueriesKeys } from '@/constants/queriesKeys';
import { getProjectRatesApiEndpoint } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { toNullableString } from '@/helpers/nullable';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { getRatesSortField } from '@/pages/ProfitAndLoss/components/RatesTab/constants';
import { ProfitAndLossFilters } from '@/pages/ProfitAndLoss/types';
import { ServerResponseError } from '@/queries/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { GetProjectRatesInput, GetRatesResponse } from '@/types/rates';

export const useGetRates = (
	top: number,
	page: number,
	order: SortDirection,
	orderBy: string,
	year: number,
	month: number,
	filters: ProfitAndLossFilters,
	allDepartmentsLength: number,
	options?: PatchedQueryOptions<
		GetRatesResponse,
		AxiosError<ServerResponseError>
	>
) => {
	const { id, user, department, project, client, legalEntity } = filters;

	return useQueryWithCallback<
		GetRatesResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			ratesQueriesKeys.getRates,
			page,
			top,
			order,
			orderBy,
			year,
			month,
			id,
			user,
			department,
			project,
			client,
			legalEntity,
			allDepartmentsLength,
		],
		queryFn: async () => {
			try {
				const departmentIds =
					!department.length || department.length === allDepartmentsLength
						? null
						: department;

				const body: GetProjectRatesInput = {
					userId: toNullableString(id),
					userFullName: toNullableString(user),
					projectName: toNullableString(project),
					clientName: toNullableString(client),
					clientLegalEntity: toNullableString(legalEntity),
					departmentIds,
					orderingField: getRatesSortField(orderBy),
					isDescendingOrdering: order === SortDirection.DESC,
				};

				const { data } = await axios.post<GetRatesResponse>(
					getProjectRatesApiEndpoint(year, month, top, page),
					body
				);

				return data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		refetchOnMount: true,
		...options,
	});
};

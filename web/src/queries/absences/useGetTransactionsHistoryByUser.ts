import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { getTransactionsHistoryByUserIdEndpoint } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import {
	AbsenceTransactionHistoryResponse,
	GetTransactionHistoryQueryParams,
} from '@/types/absences';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetTransactionsHistoryByUser = (
	params: GetTransactionHistoryQueryParams,
	options?: PatchedQueryOptions<
		AbsenceTransactionHistoryResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback({
		queryKey: [absencesQueriesKeys.getUserTransactionsHistory, params],
		queryFn: async () => {
			const { top, skip, orderBy, filters, userId } = params;
			const { inquiryTemplateName, year } = filters ?? {};

			const query = buildQuery({
				top,
				skip,
				orderBy,
				filter: {
					and: [
						inquiryTemplateName &&
							`cast(inquiryTemplateName, Edm.String) eq cast('${replaceSpecialCharacters(
								inquiryTemplateName
							)}', Edm.String)`,
					],
				},
			});

			try {
				const response = await axios.get<AbsenceTransactionHistoryResponse>(
					`${getTransactionsHistoryByUserIdEndpoint(userId!)}/${query}`,
					{
						params: {
							year,
						},
					}
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		refetchOnWindowFocus: false,
		...options,
	});

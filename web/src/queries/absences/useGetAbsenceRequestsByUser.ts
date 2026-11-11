import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { getAbsencesRequestsByUserIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ByPageResponse, ServerResponseError } from '@/queries/types';
import { convertFilterArrayToQueryParam } from '@/queries/utils';
import { convertDateFromToValueToQueryParam } from '@/queries/utils/convertFromToValueToQueryParam';
import {
	AbsenceRequest,
	AbsenceRequestStatus,
	GetAbsenceRequestQueryParams,
} from '@/types/absences';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetAbsenceRequestsByUser = (
	params: GetAbsenceRequestQueryParams,
	options?: PatchedQueryOptions<
		ByPageResponse<AbsenceRequest>,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback({
		queryKey: [absencesQueriesKeys.getUserAbsencesRequest, params],
		queryFn: async () => {
			const { top, skip, orderBy, filters, userId, numberOfAbsences } = params;
			const { inquiryTemplateName, id, status, dateTo, dateFrom } =
				filters ?? {};

			const query = buildQuery({
				top,
				skip,
				orderBy,
				filter: {
					and: [
						id && `id eq ${id}`,
						inquiryTemplateName && {
							inquiryTemplateName: convertFilterArrayToQueryParam(
								inquiryTemplateName,
								numberOfAbsences
							),
						},
						status && {
							status: convertFilterArrayToQueryParam(
								status,
								Object.keys(AbsenceRequestStatus).length
							),
						},
						...convertDateFromToValueToQueryParam(dateTo, 'dateTo'),
						...convertDateFromToValueToQueryParam(dateFrom, 'dateFrom'),
					],
				},
			});

			try {
				const response = await axios.get<ByPageResponse<AbsenceRequest>>(
					`${getAbsencesRequestsByUserIdEndpoint(userId!)}/${query}`
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		refetchOnWindowFocus: false,
		...options,
	});

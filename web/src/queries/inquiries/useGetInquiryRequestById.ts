import axios, { AxiosError } from 'axios';

import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { MAX_RETRY_ATTEMPTS } from '@/constants/queryParams';
import { getInquiryByIdEndpoint } from '@/constants/queryPaths';
import { ERROR_BAD_REQUEST } from '@/constants/requestErrorCodes';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { AbsenceRequest } from '@/types/absences';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetInquiryRequestById = (
	id?: number | string | null,
	options?: PatchedQueryOptions<AbsenceRequest, AxiosError<ServerResponseError>>
) =>
	useQueryWithCallback({
		queryKey: [absencesQueriesKeys.getAbsencesRequestById, `${id}`],
		queryFn: () =>
			axios
				.get<AbsenceRequest>(getInquiryByIdEndpoint(id!))
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		retry: (failureCount, error) => {
			if (error.code === ERROR_BAD_REQUEST) {
				return false;
			}

			return failureCount < MAX_RETRY_ATTEMPTS;
		},
		...options,
	});

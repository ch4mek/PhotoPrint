import axios, { AxiosError } from 'axios';

import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { GET_ABSENCES_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { Absence } from '@/types/absences';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetAbsences = (
	options?: PatchedQueryOptions<Absence[], AxiosError<ServerResponseError>>
) =>
	useQueryWithCallback({
		queryKey: [absencesQueriesKeys.getAbsencesData],
		queryFn: () =>
			axios
				.get<Absence[]>(GET_ABSENCES_API_ENDPOINT)
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});

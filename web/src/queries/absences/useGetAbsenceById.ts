import axios, { AxiosError } from 'axios';

import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { getAbsenceByIdApiEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { Absence } from '@/types/absences';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetAbsenceById = (
	id?: number | string | null,
	options?: PatchedQueryOptions<Absence, AxiosError<ServerResponseError>>
) =>
	useQueryWithCallback({
		queryKey: [absencesQueriesKeys.getAbsenceByIdData, id],
		queryFn: () =>
			axios
				.get<Absence>(getAbsenceByIdApiEndpoint(id!))
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});

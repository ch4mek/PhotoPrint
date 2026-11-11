import axios, { AxiosError } from 'axios';

import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { GET_USERS_FOR_ABSENCES_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { UsersAssignmentResponse } from '@/types/absences';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetUsersForAssignments = (
	absenceId: number | null,
	agreementId: number | null,
	options?: PatchedQueryOptions<
		UsersAssignmentResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback({
		queryKey: [
			absencesQueriesKeys.getUsersForAssignmentsData,
			absenceId,
			agreementId,
		],
		queryFn: () =>
			axios
				.post<UsersAssignmentResponse>(GET_USERS_FOR_ABSENCES_API_ENDPOINT, {
					templateId: absenceId,
					agreementId,
				})
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});

import axios, { AxiosError } from 'axios';

import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { geAbsencesByUserIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { UserAbsenceItem } from '@/types/absences';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetUserAbsences = (
	userId: string | number,
	options?: PatchedQueryOptions<
		UserAbsenceItem[],
		AxiosError<ServerResponseError>
	>,
	isPopUpQuery: boolean | undefined = false
) =>
	useQueryWithCallback({
		queryKey: [
			isPopUpQuery
				? absencesQueriesKeys.getUserAbsencesPopup
				: absencesQueriesKeys.getUserAbsences,
			`${userId}`,
		],
		queryFn: () =>
			axios
				.get<UserAbsenceItem[]>(geAbsencesByUserIdEndpoint(userId))
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});

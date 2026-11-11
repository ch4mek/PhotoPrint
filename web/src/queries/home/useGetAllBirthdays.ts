import axios, { AxiosError } from 'axios';

import { homeQueriesKeys } from '@/constants/queriesKeys';
import { GET_ALL_BIRTHDAYS_API_ENDPOINT } from '@/constants/queryPaths';
import { defaultSearchOptions } from '@/constants/search';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { AllBirthdaysResponseItem } from '@/types/home';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetAllBirthdays = (
	searchValue: string,
	options?: PatchedQueryOptions<
		AllBirthdaysResponseItem[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback({
		queryKey: [homeQueriesKeys.getAllBirthdays, searchValue],
		queryFn: () =>
			axios
				.get<AllBirthdaysResponseItem[]>(GET_ALL_BIRTHDAYS_API_ENDPOINT, {
					params: {
						keyword:
							searchValue.length > defaultSearchOptions.maxCharacters
								? ''
								: searchValue,
					},
				})
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});

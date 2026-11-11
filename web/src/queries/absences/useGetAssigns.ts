import {
	DefinedInitialDataInfiniteOptions,
	useInfiniteQuery,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ByPageResponse, ServerResponseError } from '../types';

import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { getAssignsEndpoint } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { AgreementAssignsItem } from '@/types/absences';

type TQueryFnData = ByPageResponse<AgreementAssignsItem>;
type TError = AxiosError<ServerResponseError>;
type TData = AgreementAssignsItem[];
type TQueryKey = [string, string, number, string];
type TPageParam = number;
type QueryOptions = DefinedInitialDataInfiniteOptions<
	TQueryFnData,
	TError,
	TData,
	TQueryKey,
	TPageParam
>;

export const useGetAssigns = (
	id: number,
	search: string,
	itemsPerPage: number,
	entityName: string,
	options?: QueryOptions
) =>
	useInfiniteQuery<TQueryFnData, TError, TData, TQueryKey, TPageParam>({
		queryKey: [absencesQueriesKeys.getAssigns, entityName, id, search],
		queryFn: async ({ pageParam }) => {
			const query = buildQuery({
				top: itemsPerPage,
				skip: pageParam * itemsPerPage,
				filter: {
					and: [
						search &&
							`contains(tolower(userFullName), tolower('${replaceSpecialCharacters(
								search
							)}'))`,
					],
				},
				orderBy: `UserId ${SortDirection.ASC}`,
			});

			try {
				const response = await axios.get<ByPageResponse<AgreementAssignsItem>>(
					`${getAssignsEndpoint(entityName, id)}${query}`
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		initialPageParam: 0,
		getNextPageParam: (lastPage) => {
			const { items, page, pageSize, total } = lastPage;

			const isLastPage = items.length < pageSize || total <= page * pageSize;

			return isLastPage ? null : page;
		},
		select: ({ pages }) => pages.flatMap(({ items }) => items),
		...options,
	});

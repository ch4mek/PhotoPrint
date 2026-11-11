import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { GetUserContractsResponse, UserContract } from '../types';

import { GET_USER_CONTRACTS_API_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetContractById = (
	id: number,
	queryOptions?: PatchedQueryOptions<
		UserContract,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<UserContract, AxiosError<ServerResponseError>>({
		queryKey: [id],
		queryFn: async () => {
			const searchFilter = {
				and: [
					`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
						String(id)
					)}', Edm.String)`,
				],
			};

			try {
				const query = buildQuery({
					filter: searchFilter,
					top: 1,
					skip: 0,
				});
				const res = await axios.get<GetUserContractsResponse>(
					`${GET_USER_CONTRACTS_API_ENDPOINT}${query}`
				);

				return res.data.items[0];
			} catch (error) {
				return Promise.reject(error);
			}
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		enabled: Boolean(id),
		...queryOptions,
	});

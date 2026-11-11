import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ClientQuerieKeys } from '@/constants/queriesKeys';
import { GET_CLIENTS_API_ENDPOINT } from '@/constants/queryPaths';
import { getKeywordQuery } from '@/helpers/queries';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ClientsLookupResponse } from '@/types/clients';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

interface UseClientsLookupOptions {
	search?: string | null;
	queryOptions?: PatchedQueryOptions<Array<ClientsLookupResponse>, AxiosError>;
	isDeleted?: boolean;
	onlyManagedClients?: boolean;
	isActive?: boolean;
}

export const useClientsLookup = ({
	search = null,
	queryOptions,
	isDeleted,
	onlyManagedClients,
	isActive,
}: UseClientsLookupOptions = {}) => {
	const query = getKeywordQuery({
		search,
		isDeleted,
		onlyManagedClients,
		isActive,
	});

	return useQueryWithCallback<Array<ClientsLookupResponse>, AxiosError>({
		...queryOptions,
		queryKey: [ClientQuerieKeys.clientsLookup, search, onlyManagedClients],
		queryFn: async () => {
			const { data } = await axios.get<ClientsLookupResponse[]>(
				`${GET_CLIENTS_API_ENDPOINT}/lookup${query}`
			);

			return data;
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
	});
};

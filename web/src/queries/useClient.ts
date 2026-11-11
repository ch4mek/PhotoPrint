import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';

import { ClientQuerieKeys } from '@/constants/queriesKeys';
import { GET_CLIENTS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ClientResponse } from '@/types/clients';

export const useClient = (id: number | string | undefined) =>
	useQueryWithCallback({
		queryKey: [ClientQuerieKeys.clients, id],
		queryFn: () =>
			axios
				.get<ClientResponse>(`${GET_CLIENTS_API_ENDPOINT}/${id}`)
				.then((res) => {
					res.data.comments = res.data.comments ?? '';

					return res.data;
				}),

		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		enabled: Boolean(id),
	});

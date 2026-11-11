import axios, { AxiosError } from 'axios';

import { HttpStatuses } from '@/constants/httpStatuses';
import { payloadQueriesKeys } from '@/constants/queriesKeys';
import { getPayloadTotalEndpoint } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { BodyFilters, TotalMonth } from '@/pages/Payload/types';

export const useGetPayloadTotal = (year: number, body: BodyFilters) =>
	useQueryWithCallback<TotalMonth[], AxiosError>({
		queryKey: [
			payloadQueriesKeys.getTotalPayloadData,
			year,
			JSON.stringify(body),
		],
		queryFn: async () => {
			const response = await axios.post<TotalMonth[]>(
				getPayloadTotalEndpoint(year),
				body
			);

			return response.data;
		},
		refetchOnWindowFocus: false,
		retry: (_, error) => error.status !== HttpStatuses.NotFound,
	});

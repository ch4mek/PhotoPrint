import axios, { AxiosError } from 'axios';

import { payloadQueriesKeys } from '@/constants/queriesKeys';
import { getPayloadApiEndpoint } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { BodyFilters, PayloadResponse } from '@/pages/Payload/types';

export const useGetPayloadByUser = (year: number, filterBody: BodyFilters) =>
	useQueryWithCallback<PayloadResponse, AxiosError, PayloadResponse>({
		queryKey: [
			payloadQueriesKeys.getPayloadDataByUser,
			year,
			filterBody.userId,
		],
		queryFn: async () =>
			axios
				.post<PayloadResponse>(getPayloadApiEndpoint(year), filterBody)
				.then((res) => res.data),
		refetchOnWindowFocus: false,
		enabled: !!filterBody.userId,
	});

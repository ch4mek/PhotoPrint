import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ratesQueriesKeys } from '@/constants/queriesKeys';
import { getExchangeRatesApiEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { ExchangeRatesResponse } from '@/types/exchangeRates';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetExchangeRates = (
	date: string,
	options?: PatchedQueryOptions<
		ExchangeRatesResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<ExchangeRatesResponse, AxiosError<ServerResponseError>>({
		queryKey: [ratesQueriesKeys.getExchangeRates, date],
		queryFn: async () => {
			try {
				const res = await axios.get<ExchangeRatesResponse>(
					getExchangeRatesApiEndpoint(date)
				);

				return res.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		refetchOnMount: true,
		...options,
	});

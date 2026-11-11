import axios, { AxiosError } from 'axios';

import { currenciesQueriesKeys } from '@/constants/queriesKeys';
import { GET_CURRENCIES_LOOKUP_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { CurrencyLookupItem } from '@/types/currency';

export const useGetCurrenciesLookup = () =>
	useQueryWithCallback<CurrencyLookupItem[], AxiosError>({
		queryKey: [currenciesQueriesKeys.getCurrenciesLookup],
		queryFn: () =>
			axios
				.get<CurrencyLookupItem[]>(GET_CURRENCIES_LOOKUP_API_ENDPOINT)
				.then((res) => res.data),
		refetchOnWindowFocus: false,
	});

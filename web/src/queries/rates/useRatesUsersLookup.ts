import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ratesQueriesKeys } from '@/constants/queriesKeys';
import { getFinanceUsersLookupApiEndpoint } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { FinanceUser } from '@/types/rates';

export const useRatesUsersLookup = (year: number, month: number) =>
	useQueryWithCallback<Array<FinanceUser>, AxiosError>({
		queryKey: [ratesQueriesKeys.getRatesUsers, year, month],
		queryFn: () =>
			axios
				.get<Array<FinanceUser>>(getFinanceUsersLookupApiEndpoint(year, month))
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});

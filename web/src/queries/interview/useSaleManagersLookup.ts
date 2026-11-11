import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_SALE_MANAGERS_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { LookupSaleManagersResponse } from '@/types/interview';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

interface UseSaleManagersLookupOptions {
	hasInterviews?: boolean;
	onlyActive?: boolean;
	includeTopManagement?: boolean;
	queryOptions?: PatchedQueryOptions<
		Array<LookupSaleManagersResponse>,
		AxiosError
	>;
}

export const useSaleManagersLookup = ({
	hasInterviews,
	onlyActive,
	includeTopManagement = true,
	queryOptions,
}: UseSaleManagersLookupOptions) =>
	useQueryWithCallback<Array<LookupSaleManagersResponse>, AxiosError>({
		...queryOptions,
		queryKey: ['sale-managers-lookup', hasInterviews, onlyActive],
		queryFn: () =>
			axios
				.get<Array<LookupSaleManagersResponse>>(GET_SALE_MANAGERS_ENDPOINT, {
					params: { hasInterviews, onlyActive, includeTopManagement },
				})
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});

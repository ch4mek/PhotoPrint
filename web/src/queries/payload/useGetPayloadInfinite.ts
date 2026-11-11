import { useInfiniteQuery, type InfiniteData } from '@tanstack/react-query';
import type { AxiosError } from 'axios';
import axios from 'axios';

import { HttpStatuses } from '@/constants/httpStatuses';
import { payloadQueriesKeys } from '@/constants/queriesKeys';
import { getPayloadApiEndpoint } from '@/constants/queryPaths';
import { PAGE_SIZE } from '@/pages/Payload/contants';
import type { BodyFilters, PayloadResponse } from '@/pages/Payload/types';

import { PayloadQueryParams } from './types';

export const useGetPayloadInfinite = (
	{
		year,
		pageSize = PAGE_SIZE,
		order,
		orderBy,
		specializationTitle,
	}: PayloadQueryParams,
	body: BodyFilters
) =>
	useInfiniteQuery<
		PayloadResponse,
		AxiosError,
		InfiniteData<PayloadResponse, number>,
		[string, PayloadQueryParams, string],
		number
	>({
		queryKey: [
			payloadQueriesKeys.getPayloadData,
			{
				year,
				pageSize,
				order,
				orderBy,
				specializationTitle,
			},
			JSON.stringify(body),
		],
		initialPageParam: 0,
		queryFn: async ({ pageParam }) =>
			axios
				.post<PayloadResponse>(
					getPayloadApiEndpoint(year, pageSize, pageParam),
					body
				)
				.then((response) => response.data),
		getNextPageParam: (lastPage, _allPages, lastPageParam) =>
			lastPage.users.length < pageSize ? undefined : lastPageParam + 1,
		refetchOnWindowFocus: false,
		retry: (_, error) => error.status !== HttpStatuses.NotFound,
	});

import { InfiniteData, QueryKey, useQueryClient } from '@tanstack/react-query';
import { useEffect } from 'react';

export function useTrimInfiniteQueryOnUnmount(queryKey: QueryKey) {
	const queryClient = useQueryClient();

	const serializedKey = JSON.stringify(queryKey);

	useEffect(
		() => () => {
			queryClient.setQueryData<InfiniteData<unknown>>(
				queryKey,
				(cachedData) => {
					if (!cachedData) {
						return cachedData;
					}

					return {
						pages: [cachedData.pages[0]],
						pageParams: [cachedData.pageParams[0]],
					};
				}
			);
		},
		[queryClient, serializedKey]
	);
}

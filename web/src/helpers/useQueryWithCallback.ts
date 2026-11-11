import { QueryKey, useQuery, UseQueryResult } from '@tanstack/react-query';
import { useEffect } from 'react';

import { UseQueryWithCallbackProps } from '@/types/patchedQueryOptions';

export const useQueryWithCallback = <
	TQueryFnData = unknown,
	TError = Error,
	TData = TQueryFnData,
	TQueryKey extends QueryKey = readonly unknown[]
>({
	onError,
	onSuccess,
	...useQueryProps
}: UseQueryWithCallbackProps<
	TQueryFnData,
	TError,
	TData,
	TQueryKey
>): UseQueryResult<TData, TError> => {
	const useQueryResult = useQuery(useQueryProps);
	const { data, error } = useQueryResult;

	useEffect(() => {
		if (data && onSuccess) {
			onSuccess(data);
		}
	}, [data]);

	useEffect(() => {
		if (error && onError) {
			onError(error);
		}
	}, [error]);

	return useQueryResult;
};

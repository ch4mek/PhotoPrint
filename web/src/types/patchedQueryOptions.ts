import { QueryKey, UseQueryOptions } from '@tanstack/react-query';

export type UseQueryWithCallbackProps<
	TQueryFnData = unknown,
	TError = Error,
	TData = TQueryFnData,
	TQueryKey extends QueryKey = readonly unknown[]
> = UseQueryOptions<TQueryFnData, TError, TData, TQueryKey> & {
	onError?: (error: TError) => void;
	onSuccess?: (data: TData) => void;
};

export type PatchedQueryOptions<
	TQueryFnData = unknown,
	TError = Error,
	TData = TQueryFnData,
	TQueryKey extends QueryKey = readonly unknown[]
> = Partial<UseQueryWithCallbackProps<TQueryFnData, TError, TData, TQueryKey>>;

import { QueryKey, useQueryClient } from '@tanstack/react-query';

export const useOptimisticUpdate = () => {
	const queryClient = useQueryClient();

	const getPreviousData = async <T>({
		queryKey,
		newValue,
	}: {
		queryKey: QueryKey;
		newValue: T;
	}): Promise<T | undefined> => {
		await queryClient.cancelQueries({ queryKey });

		const previousData = queryClient.getQueryData<T>(queryKey);

		queryClient.setQueryData(queryKey, (oldValue: T | undefined) => ({
			...oldValue,
			...newValue,
		}));

		return previousData;
	};

	return { getPreviousData };
};

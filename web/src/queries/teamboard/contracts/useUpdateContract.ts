import {
	useMutation,
	useQueryClient,
	UseMutationOptions,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { UpdateUserContract } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_CONTRACTS_API_ENDPOINT } from '@/constants/queryPaths';
import { ServerResponseError } from '@/queries/types';

export const useUpdateContract = (
	options: UseMutationOptions<
		UpdateUserContract,
		AxiosError<ServerResponseError>,
		UpdateUserContract
	>
) => {
	const queryClient = useQueryClient();

	return useMutation<
		UpdateUserContract,
		AxiosError<ServerResponseError>,
		UpdateUserContract
	>({
		...options,
		onSuccess: (...args) => {
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.userContracts],
			});

			return options.onSuccess?.(...args);
		},
		mutationFn: ({
			contractId,
			contractFrom,
			contractTo,
			fireDate,
			fireType,
			fireReason,
			fireDescription,
		}) =>
			axios
				.put(GET_USER_CONTRACTS_API_ENDPOINT, {
					id: contractId,
					contractFrom,
					contractTo,
					fireDate,
					fireType,
					fireReason: !fireReason ? null : fireReason,
					fireDescription: !fireDescription ? null : fireDescription,
				})
				.then((res) => res.data)
				.catch((error: AxiosError<ServerResponseError>) => {
					throw error;
				}),
	});
};

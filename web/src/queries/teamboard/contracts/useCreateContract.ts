import {
	useMutation,
	useQueryClient,
	UseMutationOptions,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { CreateUserContract } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_CONTRACTS_API_ENDPOINT } from '@/constants/queryPaths';
import { ServerResponseError } from '@/queries/types';

export const useCreateContract = (
	options: UseMutationOptions<
		CreateUserContract,
		AxiosError<ServerResponseError>,
		CreateUserContract
	>
) => {
	const queryClient = useQueryClient();

	return useMutation<
		CreateUserContract,
		AxiosError<ServerResponseError>,
		CreateUserContract
	>({
		...options,
		onSuccess: (...args) => {
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.userContracts],
			});

			return options.onSuccess?.(...args);
		},
		mutationFn: ({
			userId,
			contractFrom,
			contractTo,
			fireDate,
			fireType,
			fireReason,
			fireDescription,
		}) =>
			axios
				.post(GET_USER_CONTRACTS_API_ENDPOINT, {
					userId,
					contractFrom,
					contractTo,
					fireDate,
					fireType,
					fireReason: !fireReason ? null : fireReason,
					fireDescription: !fireDescription ? null : fireDescription,
				})
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
};

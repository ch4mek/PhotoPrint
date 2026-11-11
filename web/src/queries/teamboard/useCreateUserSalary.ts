import {
	useMutation,
	useQueryClient,
	UseMutationOptions,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_SALARY_ENDPOINT } from '@/constants/queryPaths';
import { UpdateUserSalary } from '@/forms/TeamboardForm/UserSalary/types';

export const useCreateUserSalary = (
	options: UseMutationOptions<
		UpdateUserSalary,
		AxiosError<ServerResponseError>,
		UpdateUserSalary
	>
) => {
	const queryClient = useQueryClient();

	return useMutation<
		UpdateUserSalary,
		AxiosError<ServerResponseError>,
		UpdateUserSalary
	>({
		...options,
		onSuccess: (...args) => {
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.getUserSalaries],
			});

			return options.onSuccess?.(...args);
		},
		mutationFn: (item) =>
			axios
				.post(GET_USER_SALARY_ENDPOINT, item)
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
};

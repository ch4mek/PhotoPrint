import {
	useMutation,
	useQueryClient,
	UseMutationOptions,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { getUserSoftSkillsEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { CreateUserSoftSkill, ServerResponseError } from '@/queries/types';

export const useCreateUserSoftSkill = (
	userId?: number,
	options?: UseMutationOptions<
		CreateUserSoftSkill,
		AxiosError<ServerResponseError>,
		CreateUserSoftSkill
	>
) => {
	const queryClient = useQueryClient();

	return useMutation<
		CreateUserSoftSkill,
		AxiosError<ServerResponseError>,
		CreateUserSoftSkill
	>({
		...options,
		onSuccess: (...args) => {
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.userSoftSkill],
			});
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.personalData],
			});

			return options?.onSuccess?.(...args);
		},
		mutationFn: (data) =>
			axios
				.post(getUserSoftSkillsEndpoint(userId), data)
				.then((res) => res.data)
				.catch(handleRequestError),
	});
};

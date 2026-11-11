import {
	useMutation,
	useQueryClient,
	UseMutationOptions,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError, UpdateUserSoftSkill } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { getUpdateUserSoftSkillEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';

export const useUpdateUserSoftSkill = (
	snapshotId?: number | string,
	options?: UseMutationOptions<
		UpdateUserSoftSkill,
		AxiosError<ServerResponseError>,
		UpdateUserSoftSkill
	>
) => {
	const queryClient = useQueryClient();

	return useMutation<
		UpdateUserSoftSkill,
		AxiosError<ServerResponseError>,
		UpdateUserSoftSkill
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
				.put(getUpdateUserSoftSkillEndpoint(snapshotId), data)
				.then((res) => res.data)
				.catch(handleRequestError),
	});
};

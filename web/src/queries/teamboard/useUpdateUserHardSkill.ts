import {
	useMutation,
	useQueryClient,
	UseMutationOptions,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import {
	NotificationType,
	USER_HARD_SKILL_UPDATED,
} from '@/constants/notifications';
import {
	TeamboardQueriesKeys,
	payloadQueriesKeys,
} from '@/constants/queriesKeys';
import { GET_USER_SPECIALIZATIONS_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';

import { UpdateUserHardSkills } from './types';

export const useUpdateUserHardSkill = (
	toast: ToastContextValues,
	options: UseMutationOptions<
		UpdateUserHardSkills,
		AxiosError<ServerResponseError>,
		UpdateUserHardSkills
	>
) => {
	const queryClient = useQueryClient();

	return useMutation<
		UpdateUserHardSkills,
		AxiosError<ServerResponseError>,
		UpdateUserHardSkills
	>({
		...options,
		onSuccess: (...args) => {
			queryClient.invalidateQueries({
				queryKey: [payloadQueriesKeys.getPayloadData],
			});
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.getUserSpecializationsSnapshots],
			});
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.teamboardUsersLookup],
			});

			return options.onSuccess?.(...args);
		},
		mutationFn: (props) =>
			axios
				.patch(GET_USER_SPECIALIZATIONS_ENDPOINT, props)
				.then((res) => {
					toast.open(
						createToast(NotificationType.SUCCESS, USER_HARD_SKILL_UPDATED)
					);

					return res.data;
				})
				.catch((error) => {
					throw error;
				}),
	});
};

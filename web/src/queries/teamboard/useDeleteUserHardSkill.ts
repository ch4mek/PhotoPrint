import {
	UseMutationOptions,
	useMutation,
	useQueryClient,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import {
	NotificationType,
	SPECIALIZATION_DELETED,
} from '@/constants/notifications';
import {
	TeamboardQueriesKeys,
	payloadQueriesKeys,
} from '@/constants/queriesKeys';
import { GET_USER_SPECIALIZATIONS_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import { UserHardSkillById } from '@/types/teamboard';

export const useDeleteUserHardSkill = (
	toast: ToastContextValues,
	options: UseMutationOptions<
		UserHardSkillById,
		AxiosError<ServerResponseError>,
		number
	>
) => {
	const queryClient = useQueryClient();

	return useMutation({
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
		mutationFn: async (id: number) => {
			try {
				const { data } = await axios.delete(
					`${GET_USER_SPECIALIZATIONS_ENDPOINT}/${id}`
				);

				toast.open(
					createToast(NotificationType.SUCCESS, SPECIALIZATION_DELETED)
				);

				return data;
			} catch (error) {
				return Promise.reject(error);
			}
		},
	});
};

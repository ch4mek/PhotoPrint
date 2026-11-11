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
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { UserSpecialization } from '@/types/teamboard';

import { UpdateSnapshotSpecializationParams } from './types';

export const useUpdateUserSpecializationSnapshot = (
	toast: ToastContextValues,
	options?: UseMutationOptions<
		UserSpecialization,
		AxiosError<ServerResponseError>,
		UpdateSnapshotSpecializationParams
	>
) => {
	const queryClient = useQueryClient();

	return useMutation<
		UserSpecialization,
		AxiosError<ServerResponseError>,
		UpdateSnapshotSpecializationParams
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
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.personalData],
			});

			return options?.onSuccess?.(...args);
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
				.catch(handleRequestError),
	});
};

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

import { AddSpecializationToUserParams } from './types';

export const useAddSpecializationForUser = (
	toast: ToastContextValues,
	options: UseMutationOptions<
		AddSpecializationToUserParams,
		AxiosError<ServerResponseError>,
		AddSpecializationToUserParams
	>
) => {
	const queryClient = useQueryClient();

	return useMutation<
		AddSpecializationToUserParams,
		AxiosError<ServerResponseError>,
		AddSpecializationToUserParams
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

			return options.onSuccess?.(...args);
		},
		mutationFn: (params) =>
			axios
				.post(GET_USER_SPECIALIZATIONS_ENDPOINT, params)
				.then((res) => {
					toast.open(
						createToast(NotificationType.SUCCESS, USER_HARD_SKILL_UPDATED)
					);

					return res.data;
				})
				.catch(handleRequestError),
	});
};

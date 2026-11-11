import {
	useMutation,
	useQueryClient,
	UseMutationOptions,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';
import { getPrimarySpecializationSetSuccessText } from '../utils/getPrimarySpecializationSetSuccessText';

import { NotificationType } from '@/constants/notifications';
import {
	TeamboardQueriesKeys,
	payloadQueriesKeys,
} from '@/constants/queriesKeys';
import { GET_USER_PRIMARY_SPECIALIZATIONS_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { FullHardSkillInfo, UserHardSkillLookup } from '@/types/hardSkills';

import { UpdateUserPrimarySpecialization } from './types';

export const useUpdateUserPrimarySpecialization = (
	toast: ToastContextValues,
	options: UseMutationOptions<
		UserHardSkillLookup<FullHardSkillInfo>,
		AxiosError<ServerResponseError>,
		UpdateUserPrimarySpecialization
	>
) => {
	const queryClient = useQueryClient();

	return useMutation<
		UserHardSkillLookup<FullHardSkillInfo>,
		AxiosError<ServerResponseError>,
		UpdateUserPrimarySpecialization
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
				queryKey: [TeamboardQueriesKeys.personalData],
			});
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.teamboardUsersLookup],
			});

			return options.onSuccess?.(...args);
		},
		mutationFn: (params) =>
			axios
				.patch(GET_USER_PRIMARY_SPECIALIZATIONS_ENDPOINT, params)
				.then((res) => {
					toast.open(
						createToast(
							NotificationType.SUCCESS,
							getPrimarySpecializationSetSuccessText(
								res.data.specializationTemplateName
							)
						)
					);

					return res.data;
				})
				.catch(handleRequestError),
	});
};

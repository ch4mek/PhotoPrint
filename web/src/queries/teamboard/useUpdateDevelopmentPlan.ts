import { useMutation, useQueryClient } from '@tanstack/react-query';
import axios, { AxiosError, AxiosResponse } from 'axios';

import { ServerResponseError } from '../types';

import {
	DEVELOPMENT_PLAN_SUCCESS_MESSAGE,
	NotificationType,
} from '@/constants/notifications';
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_DEVELOPMENT_PLANS_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import {
	createOnErrorHandler,
	handleRequestError,
} from '@/helpers/requestErrorHandler';
import { UserDevelopmentPlan } from '@/types/developmentPlan';

import { UserDevelopmentPlanProps } from './types';

export const useUpdateDevelopmentPlan = (toast: ToastContextValues) => {
	const queryClient = useQueryClient();

	return useMutation<
		UserDevelopmentPlan,
		AxiosError<ServerResponseError>,
		UserDevelopmentPlanProps
	>({
		mutationFn: async (params) => {
			try {
				const { data } = await axios.put<
					UserDevelopmentPlan,
					AxiosResponse<UserDevelopmentPlan>,
					UserDevelopmentPlanProps
				>(GET_USER_DEVELOPMENT_PLANS_ENDPOINT, params);

				return data;
			} catch (err) {
				return handleRequestError(err);
			}
		},
		onError: createOnErrorHandler(toast),
		onSuccess: () => {
			toast.open(
				createToast(NotificationType.SUCCESS, DEVELOPMENT_PLAN_SUCCESS_MESSAGE)
			);
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.getUserDevelopmentPlans],
			});
		},
	});
};

import { useMutation } from '@tanstack/react-query';
import axios, { AxiosError, AxiosResponse } from 'axios';

import {
	DEVELOPMENT_PLAN_SUCCESS_MESSAGE,
	NotificationType,
} from '@/constants/notifications';
import { UPDATE_USER_DEVELOPMENT_PLAN_STATUS_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import {
	createOnErrorHandler,
	handleRequestError,
} from '@/helpers/requestErrorHandler';
import {
	DevelopmentPlanStatus,
	UserDevelopmentPlan,
} from '@/types/developmentPlan';

import { ServerResponseError } from './types';

interface UpdateDevelopmentPlanStatusMutationBody {
	developmentPlanId: number;
	developmentPlanStatus?: keyof typeof DevelopmentPlanStatus;
	confirmationDate?: string;
}

export const useUpdateDevelopmentPlanStatus = (toast: ToastContextValues) =>
	useMutation<
		UserDevelopmentPlan,
		AxiosError<ServerResponseError>,
		UpdateDevelopmentPlanStatusMutationBody
	>({
		mutationFn: async (params) => {
			try {
				const { data } = await axios.patch<
					UserDevelopmentPlan,
					AxiosResponse<UserDevelopmentPlan>,
					UpdateDevelopmentPlanStatusMutationBody
				>(UPDATE_USER_DEVELOPMENT_PLAN_STATUS_ENDPOINT, params);

				return data;
			} catch (err) {
				return handleRequestError(err);
			}
		},
		onError: createOnErrorHandler(toast),
		onSuccess: (_, params) => {
			if (params.developmentPlanStatus === DevelopmentPlanStatus.Approved) {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						DEVELOPMENT_PLAN_SUCCESS_MESSAGE
					)
				);
			}
		},
	});

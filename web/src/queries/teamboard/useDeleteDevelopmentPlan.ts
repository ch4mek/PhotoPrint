import { useMutation, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';

import { DEVELOPMENT_PLAN_TITLE } from '@/constants/developmentPlan';
import {
	getSuccessfullyDeletedMessage,
	NotificationType,
} from '@/constants/notifications';
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { getDevelopmentPlanEndpointUrl } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import {
	createOnErrorHandler,
	handleRequestError,
} from '@/helpers/requestErrorHandler';

export const useDeleteDevelopmentPlan = (toast: ToastContextValues) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: async (id: number) => {
			try {
				const response = await axios.delete(getDevelopmentPlanEndpointUrl(id));

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		onError: createOnErrorHandler(toast),
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyDeletedMessage(DEVELOPMENT_PLAN_TITLE)
				)
			);

			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.getUserDevelopmentPlans],
			});
		},
	});
};

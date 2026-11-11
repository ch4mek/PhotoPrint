import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { getEnumKeyByValue } from '@/helpers/getEnumKeyByValue';
import { useUpdateDevelopmentPlanStatus } from '@/queries/useUpdateDevelopmentPlanStatus';
import { DevelopmentPlanStatus } from '@/types/developmentPlan';

export const useSendPlanToReview = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { mutateAsync: updateDevelopmentPlanStatus, isPending } =
		useUpdateDevelopmentPlanStatus(toast);

	const handleSendToReview = useCallback(async (id: number) => {
		await updateDevelopmentPlanStatus({
			developmentPlanId: id,
			developmentPlanStatus: getEnumKeyByValue(
				DevelopmentPlanStatus,
				DevelopmentPlanStatus.UnderReview
			),
		});

		queryClient.invalidateQueries({
			queryKey: [TeamboardQueriesKeys.getUserDevelopmentPlans],
		});
	}, []);

	return {
		handleSendToReview,
		isPending,
	};
};

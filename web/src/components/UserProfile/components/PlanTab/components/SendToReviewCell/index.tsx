import { Checkbox } from '@mms/mms-ui-library/components/Checkbox';
import { useQueryClient } from '@tanstack/react-query';
import { RenderCellProps } from 'react-data-grid';

import { UserDevelopmentPlanRow } from '../../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { getEnumKeyByValue } from '@/helpers/getEnumKeyByValue';
import { useUpdateDevelopmentPlanStatus } from '@/queries/useUpdateDevelopmentPlanStatus';
import { DevelopmentPlanStatus } from '@/types/developmentPlan';

export function SendToReviewCell({
	row,
}: RenderCellProps<UserDevelopmentPlanRow>) {
	const { status, id } = row;
	const queryClient = useQueryClient();
	const toast = useToast();
	const { mutateAsync: updateDevelopmentPlanStatus } =
		useUpdateDevelopmentPlanStatus(toast);

	const handleSendToReview = async () => {
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
	};

	const isCheckboxCheckedAndDisabled =
		status === DevelopmentPlanStatus.Approved ||
		status === DevelopmentPlanStatus.UnderReview;

	return (
		<Checkbox
			checked={isCheckboxCheckedAndDisabled}
			disabled={isCheckboxCheckedAndDisabled}
			onClick={handleSendToReview}
		/>
	);
}

import { useLoadingStatus } from '@mms/mms-ui-library/components/ManagedLoader';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useCallback, useContext, useMemo } from 'react';

import { PersonalDataContext } from '../../../context/personalDataContext';

import { InfoDataContainer } from '@/components/InfoDataContainer';
import {
	NO_DATA,
	NotificationType,
	PLAN_ADDED,
	PLAN_UPDATED,
} from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { PlanForm } from '@/forms/TeamboardForm/Plan';
import { createToast } from '@/helpers/createToast';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { PLAN_CONTAINER_TITLE } from '@/pages/TeamBoardUser/components/TeamBoardUserSummaryTab/PlanSection/constants';
import { useGetPlan } from '@/queries/teamboard/useGetPlan';
import { useUpdatePlan } from '@/queries/teamboard/useUpdatePlan';
import { PlanFormValues } from '@/types/plan';

import { PlanContainerWrapper } from './styles';

export function PlanContainer() {
	const toast = useToast();
	const { handleOpen, handleClose, isOpen } = useShowModal();
	const { userId } = useContext(PersonalDataContext);

	const { data, isFetching, isLoading } = useGetPlan(Number(userId));
	const isShowLoader = isFetching || isLoading;

	useLoadingStatus(PlanContainer.name, isShowLoader);

	const updatePlanMutation = useUpdatePlan({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					data?.plan ? PLAN_UPDATED : PLAN_ADDED
				)
			);
		},
	});

	const initialValues = useMemo(() => {
		if (!updatePlanMutation.data?.plan) {
			return {
				plan: data?.plan ?? '',
			} as PlanFormValues;
		}

		return {
			plan: updatePlanMutation.data.plan,
		};
	}, [updatePlanMutation]);

	const handleEditClick = useCallback(() => {
		handleOpen(userId);
	}, [userId]);

	const InfoDataContainerProps = useMemo(
		() =>
			data?.plan ? { handleEditClick } : { handleAddClick: handleEditClick },
		[data?.plan]
	);

	return (
		<>
			<InfoDataContainer
				{...InfoDataContainerProps}
				headerTitle={PLAN_CONTAINER_TITLE}
			>
				<PlanContainerWrapper isShowLoader={isShowLoader}>
					<Typography variant="m-400">
						{updatePlanMutation.data?.plan ?? data?.plan ?? (
							<Typography variant="m-400">{NO_DATA}</Typography>
						)}
					</Typography>
				</PlanContainerWrapper>
			</InfoDataContainer>

			{isOpen && (
				<PlanForm
					onSubmit={updatePlanMutation.mutateAsync}
					handleClose={handleClose}
					initialValues={initialValues}
					userId={userId}
				/>
			)}
		</>
	);
}

import { useLoadingStatus } from '@mms/mms-ui-library/components/ManagedLoader';
import { useContext, useEffect, useState } from 'react';

import { DevelopmentPlanTable } from '../../DevelopmentPlanTable';
import { PersonalDataContext } from '../../../context/personalDataContext';
import { useDevelopmentPlanMutations } from '../../../hooks/useDevelopmentPlanMutations';
import { DevelopmentPlanMutationMode, TeamBoardTabsId } from '../../../types';
import {
	convertDevelopmentPlanResponseToRows,
	getDevelopmentPlanLoaderRows,
} from '../../../utils';
import { TeamboardSectionContainer } from '../TeamboardSectionContainer';

import { InfoDataTableWrapper } from '@/components/InfoDataContainer/InfoDataTableWrapper';
import {
	DEFAULT_PLANS_ORDER_BY_FIELD,
	developmentPlanSectionDefaultStatusFilters,
	developmentPlanSectionQueryParams,
	START_DATE_FIELD_NAME,
} from '@/constants/developmentPlan';
import { SortDirection } from '@/constants/sort';
import { DevelopmentPlanForm } from '@/forms/TeamboardForm/DevelopmentPlan';
import { DevelopmentPlanApprovingForm } from '@/forms/TeamboardForm/DevelopmentPlanApproving';
import { DevelopmentPlanDeleteConfirmation } from '@/forms/TeamboardForm/DevelopmentPlanDeleteConfirmation';
import { getOrderValueForDevelopmentPlans } from '@/helpers/getOrderValueForDevelopmentPlans';
import { useUserDevelopmentPlans } from '@/queries/useUserDevelopmentPlans';
import { DevelopmentPlanRow } from '@/types/developmentPlan';
import { TeamboardTableMode } from '@/types/teamboard';

import { HEADER_TITLE } from './constants';
import { DevelopmentSectionProps } from './types';

export function DevelopmentPlanSection({
	developmentPlans,
	setDevelopmentPlans,
	mode,
	conversationId,
}: DevelopmentSectionProps) {
	const { userId } = useContext(PersonalDataContext);
	const [rows, setRows] = useState<Array<DevelopmentPlanRow>>([]);

	const isDevelopmentTableModePopUp = mode === TeamboardTableMode.PopUp;
	const tabId =
		mode === TeamboardTableMode.Section
			? TeamBoardTabsId.DEVELOPMENT_PLAN
			: undefined;

	const orderByValue =
		mode === TeamboardTableMode.PopUp
			? getOrderValueForDevelopmentPlans(
					START_DATE_FIELD_NAME,
					SortDirection.DESC
			  )
			: getOrderValueForDevelopmentPlans(
					DEFAULT_PLANS_ORDER_BY_FIELD,
					SortDirection.ASC
			  );

	const { data, refetch, isLoading } = useUserDevelopmentPlans(
		{
			userId,
			orderBy: orderByValue,
			...developmentPlanSectionQueryParams,
			...(isDevelopmentTableModePopUp
				? { conversationId, mode }
				: {
						filters: {
							status: developmentPlanSectionDefaultStatusFilters,
						},
				  }),
		},
		{ enabled: Boolean(conversationId) || !isDevelopmentTableModePopUp }
	);

	useLoadingStatus(DevelopmentPlanSection.name, isLoading);

	const {
		id,
		isOpen,
		mutationMode,
		selectedDevelopmentPlanStartDate,
		handleCloseClick,
		handleAddClick,
		handleAddOrEditFormSubmit,
		handleApproveFormSubmit,
		getDevelopmentPlanById,
		handleDeleteDevelopmentPlan,
		isDeleteLoading,
		...developmentPlanOperations
	} = useDevelopmentPlanMutations({
		refetch,
		developmentPlans,
		setDevelopmentPlans,
		rows,
		setRows,
		mode,
		conversationId,
	});

	useEffect(() => {
		if (isLoading) {
			setRows(getDevelopmentPlanLoaderRows());

			return;
		}

		if (data?.items) {
			setRows(
				convertDevelopmentPlanResponseToRows(
					data.items,
					developmentPlanOperations
				)
			);

			return;
		}

		setRows([]);
	}, [data?.items, isLoading]);

	return (
		<>
			<TeamboardSectionContainer
				headerTitle={HEADER_TITLE}
				tabId={tabId}
				handleAddClick={handleAddClick}
			>
				<InfoDataTableWrapper>
					<DevelopmentPlanTable mode={mode} rows={rows} setRows={setRows} />
				</InfoDataTableWrapper>
			</TeamboardSectionContainer>
			{isOpen && mutationMode === DevelopmentPlanMutationMode.AddOrEdit && (
				<DevelopmentPlanForm
					id={id}
					handleClose={handleCloseClick}
					onSubmit={handleAddOrEditFormSubmit}
					getDevPlanById={
						isDevelopmentTableModePopUp ? getDevelopmentPlanById : undefined
					}
				/>
			)}
			{isOpen &&
				selectedDevelopmentPlanStartDate &&
				mutationMode === DevelopmentPlanMutationMode.Approve && (
					<DevelopmentPlanApprovingForm
						startDate={selectedDevelopmentPlanStartDate}
						handleClose={handleCloseClick}
						onSubmit={handleApproveFormSubmit}
					/>
				)}
			{isOpen &&
				selectedDevelopmentPlanStartDate &&
				mutationMode === DevelopmentPlanMutationMode.Delete && (
					<DevelopmentPlanDeleteConfirmation
						id={id}
						selectedDate={selectedDevelopmentPlanStartDate}
						handleClose={handleCloseClick}
						onSubmit={handleDeleteDevelopmentPlan}
						isLoading={isDeleteLoading}
					/>
				)}
		</>
	);
}

import { useLoadingStatus } from '@mms/mms-ui-library/components/ManagedLoader';
import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useContext, useMemo } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';
import { TeamboardUserAccessLevel } from '../../types';
import { hasAccess } from '../../utils';
import { GRID_AREA_NAMES } from '../SummaryTabLayout/constants';

import { InfoDataContainer } from '@/components/InfoDataContainer';
import { Item } from '@/components/InfoDataContainer/Item';
import {
	ContainerWrapper,
	ContentColumn,
	ContentEditWrapper,
} from '@/components/InfoDataContainer/styles';
import { DATE_FORMAT } from '@/constants/date';
import {
	EMPLOYMENT_HISTORY_UPDATED,
	NotificationType,
} from '@/constants/notifications';
import {
	dashboardQueriesKeys,
	payloadQueriesKeys,
	TeamboardQueriesKeys,
} from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { EmploymentHistoryForm } from '@/forms/TeamboardForm/EmploymentHistory';
import { EmploymentHistoryFormValues } from '@/forms/TeamboardForm/EmploymentHistory/types';
import { createToast } from '@/helpers/createToast';
import { getDateFormat } from '@/helpers/getDateFormat';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import dayjs from '@/lib/dayjs';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useEmploymentHistory } from '@/queries/employmentHistory/useEmploymentHistory';
import { useUpdateEmploymentHistory } from '@/queries/employmentHistory/useUpdateEmploymentHistory';
import { EmploymentHistoryResponse } from '@/types/employmentHistory';

import { EmploymentHistoryFields } from './constants';

export function EmploymentHistoryContainer() {
	const toast = useToast();
	const queryClient = useQueryClient();
	const { id, handleOpen, handleClose, isOpen } = useShowModal();
	const { userId, showLoader, accessLevel, setPersonalDataChanged } =
		useContext(PersonalDataContext);
	const { data, isFetching } = useEmploymentHistory(userId);

	const shouldDisplayNda = Boolean(data?.shouldDisplayNda);

	useLoadingStatus(EmploymentHistoryContainer.name, isFetching || showLoader);

	const { mutateAsync: updateEmploymentHistory } = useUpdateEmploymentHistory({
		onSuccess: (response) => {
			setPersonalDataChanged?.();
			queryClient.setQueryData<EmploymentHistoryResponse>(
				[TeamboardQueriesKeys.employmentHistory, id],
				(prevData) => {
					const {
						fireDate,
						hireDate,
						professionalActivityStartDate,
						ndaStartDate,
						ndaEndDate,
						shouldDisplayNda: shouldDisplayNdaFromResponse,
					} = response;

					const { contractEndDate, firstProjectStartDate } =
						prevData || ({} as EmploymentHistoryResponse);

					return {
						contractEndDate,
						firstProjectStartDate,
						fireDate,
						hireDate,
						professionalActivityStartDate,
						ndaStartDate:
							ndaStartDate !== undefined
								? ndaStartDate
								: prevData?.ndaStartDate,
						ndaEndDate:
							ndaEndDate !== undefined ? ndaEndDate : prevData?.ndaEndDate,
						shouldDisplayNda: shouldDisplayNdaFromResponse,
					};
				}
			);

			// Adding timeout because backend doesn't update data on time
			setTimeout(() => {
				queryClient.invalidateQueries({
					queryKey: [payloadQueriesKeys.getPayloadData],
				});
				queryClient.invalidateQueries({
					queryKey: [dashboardQueriesKeys.getProbationLookup],
				});
			}, 500);

			toast.open(
				createToast(NotificationType.SUCCESS, EMPLOYMENT_HISTORY_UPDATED)
			);
		},
		onError: createOnErrorHandler(toast),
	});

	const {
		professionalActivityStartDate,
		firstProjectStartDate,
		hireDate,
		contractEndDate,
		fireDate,
		ndaStartDate,
		ndaEndDate,
	} = data || ({} as EmploymentHistoryResponse);

	const initialValues = useMemo(() => {
		if (!data) {
			return {
				professionalActivityStartDate: null,
				hireDate: null,
				fireDate: null,
				ndaStartDate: null,
				ndaEndDate: null,
			} as EmploymentHistoryFormValues;
		}

		return {
			professionalActivityStartDate: data.professionalActivityStartDate
				? dayjs(data.professionalActivityStartDate)
				: null,
			hireDate: data.hireDate ? dayjs(data.hireDate) : null,
			fireDate: data.fireDate ? dayjs(data.fireDate) : null,
			ndaStartDate: data.ndaStartDate ? dayjs(data.ndaStartDate) : null,
			ndaEndDate: data.ndaEndDate ? dayjs(data.ndaEndDate) : null,
		};
	}, [data]);

	const handleEditClick = useCallback(() => {
		handleOpen(userId);
	}, [userId]);

	const canEditEmploymentHistory = hasAccess(accessLevel, [
		TeamboardUserAccessLevel.Full,
		TeamboardUserAccessLevel.LimitedHR,
	]);

	const fireDateItem = (
		<Item
			nameOfField={EmploymentHistoryFields.FIRE_DATE}
			value={getDateFormat(fireDate, DATE_FORMAT)}
		/>
	);

	return (
		<>
			<InfoDataContainer
				orderName={GRID_AREA_NAMES.thirdGroup}
				headerTitle="Employment history"
				handleEditClick={canEditEmploymentHistory ? handleEditClick : undefined}
			>
				<ContainerWrapper>
					<ContentEditWrapper>
						<ContentColumn>
							<Item
								nameOfField={
									EmploymentHistoryFields.PROFESSIONAL_ACTIVITY_START_DATE
								}
								value={getDateFormat(
									professionalActivityStartDate,
									DATE_FORMAT
								)}
							/>
							<Item
								nameOfField={EmploymentHistoryFields.EMPLOYMENT_DATE}
								value={getDateFormat(hireDate, DATE_FORMAT)}
							/>
							{shouldDisplayNda && (
								<>
									<Item
										nameOfField={EmploymentHistoryFields.NDA_START_DATE}
										value={getDateFormat(ndaStartDate ?? null, DATE_FORMAT)}
									/>
									<Item
										nameOfField={EmploymentHistoryFields.NDA_END_DATE}
										value={getDateFormat(ndaEndDate ?? null, DATE_FORMAT)}
									/>
								</>
							)}
							{!shouldDisplayNda && fireDateItem}
						</ContentColumn>
						<ContentColumn>
							<Item
								nameOfField={EmploymentHistoryFields.FIRST_PROJECT_START_DATE}
								value={getDateFormat(firstProjectStartDate, DATE_FORMAT)}
							/>
							<Item
								nameOfField={EmploymentHistoryFields.CONTRACT_END_DATE}
								value={getDateFormat(contractEndDate, DATE_FORMAT)}
							/>
							{shouldDisplayNda && fireDateItem}
						</ContentColumn>
					</ContentEditWrapper>
				</ContainerWrapper>
			</InfoDataContainer>

			{isOpen && (
				<EmploymentHistoryForm
					onSubmit={updateEmploymentHistory}
					{...{
						id: id as number,
						handleClose,
						initialValues,
						shouldDisplayNda,
					}}
				/>
			)}
		</>
	);
}

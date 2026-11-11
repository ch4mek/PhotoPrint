import { useLoadingStatus } from '@mms/mms-ui-library/components/ManagedLoader';
import { useCallback, useContext, useMemo } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';
import { isFullAccess } from '../../utils';

import { InfoDataContainer } from '@/components/InfoDataContainer';
import { Item } from '@/components/InfoDataContainer/Item';
import {
	ContentColumn,
	ContentEditWrapper,
} from '@/components/InfoDataContainer/styles';
import { DATE_FORMAT } from '@/constants/date';
import {
	INTERNSHIP_UPDATED,
	NotificationType,
} from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { InternshipForm } from '@/forms/TeamboardForm/Internship';
import { emptyInitialValues } from '@/forms/TeamboardForm/Internship/constants';
import { getInternshipFormValuesFromResponse } from '@/forms/TeamboardForm/Internship/utils';
import { createToast } from '@/helpers/createToast';
import { formatNullableNumber } from '@/helpers/formatNullableNumber';
import { getDateFormat } from '@/helpers/getDateFormat';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useInternship } from '@/queries/internship/useInternship';
import { useUpdateInternship } from '@/queries/internship/useUpdateInternship';

import { INTERNSHIP_SECTION_TITLE } from './constants';
import { InternshipContainerWrapper } from './styles';
import { TechnologiesField } from './TechnologiesField';
import { InternshipFields } from './types';

export function InternshipSection() {
	const toast = useToast();
	const { userId, showLoader, accessLevel, setPersonalDataChanged } =
		useContext(PersonalDataContext);
	const { id, handleOpen, handleClose, isOpen } = useShowModal<number>();
	const { data, isFetching } = useInternship(userId);

	useLoadingStatus(InternshipSection.name, isFetching || showLoader);

	const canEditInternship = isFullAccess(accessLevel);

	const { mutateAsync: updateInternship } = useUpdateInternship({
		onSuccess: () => {
			setPersonalDataChanged?.();
			toast.open(createToast(NotificationType.SUCCESS, INTERNSHIP_UPDATED));
		},
		onError: createOnErrorHandler(toast),
	});

	const initialValues = useMemo(
		() =>
			data ? getInternshipFormValuesFromResponse(data) : emptyInitialValues,
		[data]
	);

	const handleEditClick = useCallback(() => {
		handleOpen(userId);
	}, [handleOpen, userId]);

	return (
		<>
			<InfoDataContainer
				headerTitle={INTERNSHIP_SECTION_TITLE}
				handleEditClick={canEditInternship ? handleEditClick : undefined}
			>
				<InternshipContainerWrapper>
					<ContentEditWrapper>
						<ContentColumn>
							<Item
								nameOfField={InternshipFields.INTERNSHIP_SPECIALIZATION}
								value={data?.specialization}
							/>
							<Item
								nameOfField={InternshipFields.INTERNSHIP_TYPE}
								value={data?.type}
							/>
							<Item
								nameOfField={InternshipFields.ENGINEER_STAGE}
								value={formatNullableNumber(data?.engineerStage)}
							/>
							<Item
								nameOfField={InternshipFields.COMPLETED_POINTS}
								value={formatNullableNumber(data?.completedPoints)}
							/>
							<Item
								nameOfField={InternshipFields.APPROVED_POINTS}
								value={formatNullableNumber(data?.approvedPoints)}
							/>
							<Item
								nameOfField={InternshipFields.FINAL_RESULT}
								value={data?.finalResult}
							/>
						</ContentColumn>
						<ContentColumn>
							<Item
								nameOfField={InternshipFields.INTERNSHIP_START_DATE}
								value={getDateFormat(data?.startDate ?? null, DATE_FORMAT)}
							/>
							<Item
								nameOfField={InternshipFields.PLANNED_INTERNSHIP_END_DATE}
								value={getDateFormat(data?.plannedEndDate ?? null, DATE_FORMAT)}
							/>
							<TechnologiesField
								technologies={data?.userInternshipSpecializations}
							/>
						</ContentColumn>
					</ContentEditWrapper>
				</InternshipContainerWrapper>
			</InfoDataContainer>

			{isOpen && id && (
				<InternshipForm
					onSubmit={updateInternship}
					userId={id}
					{...{ initialValues, handleClose }}
				/>
			)}
		</>
	);
}

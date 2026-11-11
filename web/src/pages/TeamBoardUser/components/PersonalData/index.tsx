import { useLoadingStatus } from '@mms/mms-ui-library/components/ManagedLoader';
import { useCallback, useContext, useMemo } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';
import { TeamboardUserAccessLevel } from '../../types';
import { hasAccess, isFullAccess, isInternProfile } from '../../utils';

import { InfoDataContainer } from '@/components/InfoDataContainer';
import { Item } from '@/components/InfoDataContainer/Item';
import {
	ContainerWrapper,
	ContentColumn,
	ContentEditWrapper,
} from '@/components/InfoDataContainer/styles';
import { InfoDataLink } from '@/components/InfoDataLink';
import { DATE_FORMAT } from '@/constants/date';
import {
	NotificationType,
	PERSONAL_DATA_UPDATED,
} from '@/constants/notifications';
import { isChiefRole } from '@/constants/roles';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { PersonalDataForm } from '@/forms/TeamboardForm/PersonalData';
import { PersonalDataFormValues } from '@/forms/TeamboardForm/PersonalData/types';
import { createToast } from '@/helpers/createToast';
import { findById } from '@/helpers/findById';
import { boolToGraduateJobPlacement } from '@/helpers/formatGraduateJobPlacement';
import { getDateFormat } from '@/helpers/getDateFormat';
import { getDepartmentProfileInfoValue } from '@/helpers/getDepartmentProfileInfoValue';
import { getSeniorityAndSpecialization } from '@/helpers/getSeniorityAndSpecialization';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import dayjs from '@/lib/dayjs';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useUpdatePersonalData } from '@/queries/personalData/useUpdatePersonalData';

import { PersonalDataFields } from './constants';

export function PersonalDataContainer() {
	const toast = useToast();
	const { id, handleOpen, handleClose, isOpen } = useShowModal();
	const {
		data,
		isFetching,
		position,
		accessLevel,
		userId,
		profileKind,
		setPersonalDataChanged,
	} = useContext(PersonalDataContext);

	const isIntern = isInternProfile(profileKind);

	useLoadingStatus(PersonalDataContainer.name, isFetching);

	const {
		configurableValues: { englishLevels, departmentTypes, roleTypes },
	} = useConfiguration();

	const {
		fullName,
		fullNameRu,
		email: corporateEmail,
		personalEmail,
		personalEmailSecond,
		personalPhone,
		seniority,
		primarySpecialization,
		englishLevel,
		softSkillAverageSeniority,
		departmentFullName,
		departmentId,
		dateOfBirth,
		telegram,
		resourceGroupName,
		militaryServiceStatus,
		isGraduatedJobPlacement,
		linkedIn,
	} = data;

	const updatePersonalDataMutation = useUpdatePersonalData({
		onMutate: () => {
			setPersonalDataChanged?.();
		},
		onSuccess: () => {
			toast.open(createToast(NotificationType.SUCCESS, PERSONAL_DATA_UPDATED));
		},
		onError: createOnErrorHandler(toast),
	});

	const handleEditClick = useCallback(() => {
		handleOpen(userId);
	}, [userId]);

	const departmentValue = useMemo(
		() => findById(departmentTypes, departmentId),
		[departmentTypes, departmentId]
	);

	const specializationAndSeniorityValue = useMemo(
		() => getSeniorityAndSpecialization(seniority, primarySpecialization),
		[seniority, primarySpecialization]
	);

	const englishLevelValue = useMemo(
		() => findById(englishLevels, englishLevel),
		[englishLevel, englishLevels]
	);

	const positionValue = useMemo(
		() => findById(roleTypes, position as string),
		[roleTypes, position]
	);

	const initialValues: PersonalDataFormValues = useMemo(() => {
		if (!data) {
			return {
				firstName: '',
				lastName: '',
				firstNameRu: '',
				lastNameRu: '',
				patronymicRu: '',
				fullName: '',
				fullNameRu: '',
				dateOfBirth: null,
				position: null,
				department: null,
				group: null,
				corporateEmail: null,
				personalEmail: '',
				personalEmailSecond: '',
				linkedIn: '',
				personalPhone: '',
				telegram: null,
				englishLevel: null,
				registeredLocation: '',

				militaryServiceStatus: null,
				graduateJobPlacement: null,
			};
		}

		const { email, ...restData } = data;

		return {
			...restData,
			corporateEmail: email,
			firstNameRu: restData.firstNameRu ?? '',
			lastNameRu: restData.lastNameRu ?? '',
			patronymicRu: restData.patronymicRu ?? '',
			fullNameRu: restData.fullNameRu?.trim() || null,
			englishLevel: englishLevelValue || null,
			position: positionValue || null,
			department: departmentValue || null,
			group: resourceGroupName
				? {
						id: restData.resourceGroupId as number,
						name: resourceGroupName,
				  }
				: null,
			dateOfBirth: restData.dateOfBirth
				? dayjs(restData.dateOfBirth).startOf('day')
				: null,
			graduateJobPlacement: boolToGraduateJobPlacement(
				restData.isGraduatedJobPlacement
			),
			telegram: restData.telegram ?? '',
			linkedIn: restData.linkedIn ?? '',
			personalPhone: restData.personalPhone ?? '',
			personalEmail: restData.personalEmail ?? '',
			personalEmailSecond: restData.personalEmailSecond ?? '',
		};
	}, [
		data,
		englishLevelValue,
		positionValue,
		departmentValue,
		resourceGroupName,
	]);

	const departmentItemValue = getDepartmentProfileInfoValue(
		departmentFullName,
		resourceGroupName
	);

	const canEditPersonalInfo = hasAccess(accessLevel, [
		TeamboardUserAccessLevel.Full,
		TeamboardUserAccessLevel.LimitedHR,
	]);

	return (
		<>
			<InfoDataContainer
				headerTitle="Personal data"
				handleEditClick={canEditPersonalInfo ? handleEditClick : undefined}
			>
				<ContainerWrapper>
					<ContentEditWrapper>
						<ContentColumn>
							<Item
								nameOfField={PersonalDataFields.FULL_NAME_ENGLISH}
								value={fullName}
							/>
							<Item
								nameOfField={PersonalDataFields.FULL_NAME_RUSSIAN}
								value={fullNameRu}
							/>
							<Item
								nameOfField={PersonalDataFields.DATE_OF_BIRTH}
								value={getDateFormat(dateOfBirth, DATE_FORMAT)}
							/>
							<Item
								nameOfField={PersonalDataFields.PHONE}
								value={personalPhone}
							/>
							<Item
								nameOfField={PersonalDataFields.TELEGRAM}
								value={telegram}
							/>
							<Item
								nameOfField={PersonalDataFields.LINKEDIN}
								value={
									linkedIn && <InfoDataLink href={linkedIn} value={linkedIn} />
								}
							/>
							<Item
								nameOfField={PersonalDataFields.MILITARY_SERVICE_STATUS}
								value={militaryServiceStatus}
							/>
						</ContentColumn>
						<ContentColumn>
							<Item
								nameOfField={PersonalDataFields.GRADUATE_JOB_PLACEMENT}
								value={boolToGraduateJobPlacement(isGraduatedJobPlacement)}
							/>
							<Item
								nameOfField={PersonalDataFields.EMAIL_PERSONAL}
								value={personalEmail}
								secondValue={personalEmailSecond}
							/>
							{!isChiefRole(position) && (
								<Item
									nameOfField={PersonalDataFields.DEPARTMENT}
									value={departmentItemValue}
								/>
							)}
							{!isIntern && (
								<Item
									nameOfField={PersonalDataFields.EMAIL_CORPORATE}
									value={corporateEmail}
								/>
							)}
							{!isIntern && isFullAccess(accessLevel) && (
								<>
									<Item
										nameOfField={PersonalDataFields.HARD_SKILLS}
										value={specializationAndSeniorityValue}
									/>
									<Item
										nameOfField={PersonalDataFields.SOFT_SKILLS}
										value={softSkillAverageSeniority}
									/>
								</>
							)}

							<Item
								nameOfField={PersonalDataFields.ENGLISH_LEVEL}
								value={englishLevelValue?.name}
							/>
						</ContentColumn>
					</ContentEditWrapper>
				</ContainerWrapper>
			</InfoDataContainer>
			{isOpen && (
				<PersonalDataForm
					id={id as number}
					onSubmit={updatePersonalDataMutation.mutateAsync}
					hasFullAccess={isFullAccess(accessLevel)}
					{...{ handleClose, initialValues }}
				/>
			)}
		</>
	);
}

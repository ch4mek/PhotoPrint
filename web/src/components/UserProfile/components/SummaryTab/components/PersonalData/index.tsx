import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useContext, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { PersonalDataContext } from '../../../../context/personalDataContext';

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
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { isChiefRole } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { findById } from '@/helpers/findById';
import { boolToGraduateJobPlacement } from '@/helpers/formatGraduateJobPlacement';
import { getDateFormat } from '@/helpers/getDateFormat';
import { getDepartmentProfileInfoValue } from '@/helpers/getDepartmentProfileInfoValue';
import { getSeniorityAndSpecialization } from '@/helpers/getSeniorityAndSpecialization';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useUpdateUserLocation } from '@/queries/user/useUpdateUserLocation';
import { LocationTypeId } from '@/types/configuration';
import { PersonalDataResponse } from '@/types/personalData';

import { PersonalDataForm } from './PersonalDataForm';

export function PersonalData() {
	const toast = useToast();
	const { handleOpen, handleClose, isOpen } = useShowModal();
	const { t } = useTranslation('profile', { keyPrefix: 'personalData' });
	const { data, userId } = useContext(PersonalDataContext);
	const queryClient = useQueryClient();

	const { setUserInfo, role } = useAuth();

	const {
		configurableValues: { englishLevels },
		persistentValues: { locationTypes },
	} = useConfiguration();

	const updateUserLocation = useUpdateUserLocation({
		onSuccess: (newLocation) => {
			setUserInfo((userInfo) => ({
				...userInfo,
				location: newLocation.data as LocationTypeId,
			}));
			toast.open(createToast(NotificationType.SUCCESS, PERSONAL_DATA_UPDATED));
			queryClient.setQueryData<PersonalDataResponse>(
				[TeamboardQueriesKeys.personalData, userId],
				(prevData) =>
					({
						...prevData,
						location: newLocation.data,
					} as PersonalDataResponse)
			);
		},
		onError: createOnErrorHandler(toast),
	});

	const {
		fullName,
		fullNameRu,
		email,
		personalEmail,
		personalEmailSecond,
		personalPhone,
		seniority,
		primarySpecialization,
		englishLevel,
		softSkillAverageSeniority,
		departmentFullName,
		currentLocation,
		location,
		registeredLocation,
		dateOfBirth,
		telegram,
		resourceGroupName,
		militaryServiceStatus,
		isGraduatedJobPlacement,
		linkedIn,
	} = data;

	const handleEditClick = useCallback(() => {
		handleOpen(userId);
	}, [userId]);

	const specializationAndSeniorityValue = useMemo(
		() => getSeniorityAndSpecialization(seniority, primarySpecialization),
		[seniority, primarySpecialization]
	);

	const englishLevelValue = useMemo(
		() => findById(englishLevels, englishLevel),
		[englishLevel, englishLevels]
	);

	const departmentItemValue = getDepartmentProfileInfoValue(
		departmentFullName,
		resourceGroupName
	);

	return (
		<>
			<InfoDataContainer
				headerTitle={t('title')}
				handleEditClick={handleEditClick}
			>
				<ContainerWrapper>
					<ContentEditWrapper>
						<ContentColumn>
							<Item nameOfField={t('fullNameEnglish')} value={fullName} />
							<Item nameOfField={t('fullNameRussian')} value={fullNameRu} />
							<Item
								nameOfField={t('dateOfBirth')}
								value={getDateFormat(dateOfBirth, DATE_FORMAT)}
							/>
							<Item nameOfField={t('phone')} value={personalPhone} />
							<Item nameOfField={t('telegram')} value={telegram} />
							<Item
								nameOfField={t('linkedIn')}
								value={
									linkedIn && <InfoDataLink href={linkedIn} value={linkedIn} />
								}
							/>
							<Item nameOfField={t('location')} value={location} />
							<Item
								nameOfField={t('militaryServiceStatus')}
								value={militaryServiceStatus}
							/>
							<Item
								nameOfField={t('graduateJobPlacement')}
								value={boolToGraduateJobPlacement(isGraduatedJobPlacement)}
							/>
						</ContentColumn>
						<ContentColumn>
							<Item
								nameOfField={t('emailPersonal')}
								value={personalEmail}
								secondValue={personalEmailSecond}
							/>
							{!isChiefRole(role) && (
								<Item
									nameOfField={t('department')}
									value={departmentItemValue}
								/>
							)}
							<Item nameOfField={t('emailCorporate')} value={email} />
							<Item
								nameOfField={t('hardSkills')}
								value={specializationAndSeniorityValue}
							/>
							<Item
								nameOfField={t('softSkills')}
								value={softSkillAverageSeniority}
							/>
							<Item
								nameOfField={t('englishLevel')}
								value={englishLevelValue?.name}
							/>
							<Item
								nameOfField={t('currentLocation')}
								value={currentLocation}
							/>
							<Item
								nameOfField={t('registeredLocation')}
								value={registeredLocation}
							/>
						</ContentColumn>
					</ContentEditWrapper>
				</ContainerWrapper>
			</InfoDataContainer>
			{isOpen && userId && (
				<PersonalDataForm
					userId={userId}
					onSubmit={updateUserLocation.mutateAsync}
					handleClose={handleClose}
					initialLocation={location}
					locationTypes={locationTypes}
				/>
			)}
		</>
	);
}

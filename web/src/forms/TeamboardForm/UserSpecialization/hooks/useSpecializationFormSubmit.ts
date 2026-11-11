import { useCallback, useContext } from 'react';

import { useToast } from '@/context/index';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { useAddSpecializationForUser } from '@/queries/teamboard/useCreateUserSpecialization';
import { UserSpecializationFormValues } from '@/types/teamboard';

export const useSpecializationFormSubmit = () => {
	const toast = useToast();
	const { setPersonalDataChanged } = useContext(PersonalDataContext);

	const { mutateAsync: addSpecialization } = useAddSpecializationForUser(
		toast,
		{
			onError: createOnErrorHandler(toast),
			onSuccess: () => setPersonalDataChanged?.(),
		}
	);

	const handleSubmit = useCallback(
		(values: UserSpecializationFormValues, userId: number) => {
			const specialization = values.specialization!;

			const mappedHardSkills = specialization.hardSkills.map(
				({ info, seniority }) => ({
					id: info.id,
					seniorityLevelId: seniority?.id ?? null,
				})
			);

			return addSpecialization({
				userId,
				specializationTemplateId: specialization.id,
				hardSkills: mappedHardSkills,
				specializationSeniorityLevelId: values?.specialization!.seniority!.id,
			});
		},
		[]
	);

	return handleSubmit;
};

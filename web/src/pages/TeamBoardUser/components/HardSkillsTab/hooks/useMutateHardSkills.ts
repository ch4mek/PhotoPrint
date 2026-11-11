import { useMemo, useContext } from 'react';

import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { useAddSpecializationForUser } from '@/queries/teamboard/useCreateUserSpecialization';
import { useDeleteUserHardSkill } from '@/queries/teamboard/useDeleteUserHardSkill';
import { useUpdateUserHardSkill } from '@/queries/teamboard/useUpdateUserHardSkill';
import { useUpdateUserPrimarySpecialization } from '@/queries/teamboard/useUpdateUserPrimarySpecialization';

export const useMutateSpecializations = (id?: number | string) => {
	const toast = useToast();
	const { setPersonalDataChanged } = useContext(PersonalDataContext);

	const createHardSkillMutation = useAddSpecializationForUser(toast, {
		onSuccess: () => setPersonalDataChanged?.(),
		onError: createOnErrorHandler(toast),
	});

	const updateHardSkillMutation = useUpdateUserHardSkill(toast, {
		onSuccess: () => setPersonalDataChanged?.(),
		onError: createOnErrorHandler(toast),
	});

	const updateUserPrimarySpecialization = useUpdateUserPrimarySpecialization(
		toast,
		{
			onSuccess: () => setPersonalDataChanged?.(),
			onError: createOnErrorHandler(toast),
		}
	);

	const deleteHardSkillMutation = useDeleteUserHardSkill(toast, {
		onSuccess: () => setPersonalDataChanged?.(),
		onError: createOnErrorHandler(toast),
	});

	return useMemo(
		() => ({
			submitFunction: id
				? updateHardSkillMutation.mutateAsync
				: createHardSkillMutation.mutateAsync,
			deleteFunction: deleteHardSkillMutation.mutateAsync,
			updatePrimaryFunction: updateUserPrimarySpecialization.mutateAsync,
		}),
		[id]
	);
};

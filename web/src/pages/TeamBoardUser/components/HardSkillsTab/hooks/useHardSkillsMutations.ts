import { useContext } from 'react';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { useToast } from '@/context/index';
import { UserSpecializationsFormValues } from '@/forms/TeamboardForm/UserSpecializations/types';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import dayjs from '@/lib/dayjs';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { useCreateUserSpecializationSnapshot } from '@/queries/teamboard/useCreateSpecializationSnapshot';
import { useUpdateUserSpecializationSnapshot } from '@/queries/teamboard/useUpdateSpecializationSnapshot';

export const useHardSkillsMutations = (userId?: number) => {
	const toast = useToast();
	const { setPersonalDataChanged } = useContext(PersonalDataContext);

	const { mutateAsync: createSpecializationSnapshot } =
		useCreateUserSpecializationSnapshot(toast, {
			onSuccess: () => setPersonalDataChanged?.(),
			onError: createOnErrorHandler(toast),
		});
	const { mutateAsync: updateSpecializationSnapshot } =
		useUpdateUserSpecializationSnapshot(toast, {
			onSuccess: () => setPersonalDataChanged?.(),
			onError: createOnErrorHandler(toast),
		});

	const getSpecializationsFormSubmitFunction =
		(isEdit: boolean) =>
		(
			{ snapshotDate, userSpecializations }: UserSpecializationsFormValues,
			snapshotId: number | null
		) => {
			if (!userId) {
				return;
			}

			const requestBody = {
				userId,
				userSpecializations: userSpecializations.map(
					({ id, hardSkills, seniority: specializationSeniorityLevelId }) => ({
						specializationTemplateId: id,
						hardSkills: hardSkills.map(({ info, seniority }) => ({
							id: info.id,
							seniorityLevelId: seniority?.id ?? null,
						})),
						specializationSeniorityLevelId: specializationSeniorityLevelId!.id,
					})
				),
			};

			if (isEdit && snapshotId) {
				return updateSpecializationSnapshot({
					...requestBody,
					snapshotId,
				});
			}

			return createSpecializationSnapshot({
				...requestBody,
				snapshotDate: dayjs(snapshotDate).format(YYYY_MM_DD_DAYJS_DATE_FORMAT),
			});
		};

	return {
		getSpecializationsFormSubmitFunction,
	};
};

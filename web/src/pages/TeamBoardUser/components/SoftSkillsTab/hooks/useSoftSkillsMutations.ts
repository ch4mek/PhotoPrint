import {
	NotificationType,
	USER_SOFT_SKILLS_ARE_UPDATED,
} from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { UseSoftSkillsMutationsProps } from '@/pages/TeamBoardUser/components/SoftSkillsTab/types';
import { useCreateUserSoftSkill } from '@/queries/softSkills/useCreateUserSoftSkill';
import { useUpdateUserSoftSkill } from '@/queries/softSkills/useUpdateUserSoftSkill';

export const useSoftSkillsMutations = ({
	handleClose,
	snapshotId,
	userId,
}: UseSoftSkillsMutationsProps) => {
	const toast = useToast();

	const onSuccess = () => {
		toast.open(
			createToast(NotificationType.SUCCESS, USER_SOFT_SKILLS_ARE_UPDATED)
		);

		handleClose();
	};

	const { mutateAsync: mutateUpdateUserSoftSkill } = useUpdateUserSoftSkill(
		snapshotId,
		{
			onSuccess,
			onError: createOnErrorHandler(toast),
		}
	);

	const { mutateAsync: mutateCreateSoftSkill } = useCreateUserSoftSkill(
		userId,
		{
			onSuccess,
			onError: createOnErrorHandler(toast),
		}
	);

	return {
		mutateUpdateUserSoftSkill,
		mutateCreateSoftSkill,
	};
};

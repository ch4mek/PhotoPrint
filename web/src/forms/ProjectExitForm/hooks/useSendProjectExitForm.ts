import { useQueryClient } from '@tanstack/react-query';

import {
	NotificationType,
	PROJECT_EXIT_MESSAGES,
} from '@/constants/notifications';
import { projectExitQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useAddProjectExits } from '@/queries/projectExits/useAddProjectExits';
import { useEditProjectExits } from '@/queries/projectExits/useEditProjectExits';
import {
	AddProjectExitPayload,
	EditProjectExitPayload,
} from '@/types/projectExits';

export const useSendProjectExitForm = (handleClose: VoidFunction) => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const invalidateProjectExitQueries = () => {
		queryClient.invalidateQueries({
			queryKey: [projectExitQueriesKeys.getProjectExits],
		});
		queryClient.invalidateQueries({
			queryKey: [projectExitQueriesKeys.getProjectExitReasons],
		});
	};

	const { mutateAsync: createMutation, isPending: isLoadingCreation } =
		useAddProjectExits({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						PROJECT_EXIT_MESSAGES.CREATED_SUCCESS
					)
				);

				handleClose();
				invalidateProjectExitQueries();
			},
			onError: createOnErrorHandler(toast),
		});

	const { mutateAsync: editMutation, isPending: isLoadingEdit } =
		useEditProjectExits({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						PROJECT_EXIT_MESSAGES.EDITED_SUCCESS
					)
				);

				handleClose();
				invalidateProjectExitQueries();
			},
			onError: createOnErrorHandler(toast),
		});

	const handleAddProjectExit = async (values: AddProjectExitPayload) => {
		await createMutation(values);
	};

	const handleEditProjectExit = async (values: EditProjectExitPayload) => {
		await editMutation(values);
	};

	const isLoading = isLoadingCreation || isLoadingEdit;

	return {
		handleAddProjectExit,
		handleEditProjectExit,
		isLoading,
	};
};

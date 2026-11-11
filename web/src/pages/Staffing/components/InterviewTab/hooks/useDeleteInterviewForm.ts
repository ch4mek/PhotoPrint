import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import { INTERVIEW_DELETED_MESSAGE } from '../constants';

import { NotificationType } from '@/constants/notifications';
import { interviewQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useDeleteInterview } from '@/queries/interview/useDeleteInterview';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export const useDeleteInterviewForm = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const {
		id: itemToDelete,
		handleOpen,
		handleClose,
		isOpen,
	} = useShowModal<SimpleConfigurableConfigurationItem<number | string>>();

	const { mutateAsync: deleteMutation, isPending: isLoading } =
		useDeleteInterview({
			onSuccess: () => {
				toast.open(
					createToast(NotificationType.SUCCESS, INTERVIEW_DELETED_MESSAGE)
				);

				handleClose();

				queryClient.invalidateQueries({
					queryKey: [interviewQueriesKeys.getInterviewsData],
				});
			},
			onError: createOnErrorHandler(toast),
		});

	const handleFormSubmit = useCallback(async () => {
		if (itemToDelete?.id) {
			await deleteMutation(itemToDelete.id);
		}
	}, [deleteMutation, itemToDelete]);

	return {
		id: itemToDelete,
		isOpen,
		isLoading,
		handleOpen,
		handleClose,
		handleFormSubmit,
	};
};

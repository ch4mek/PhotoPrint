import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import {
	getSuccessfullyDeletedMessage,
	NotificationType,
} from '@/constants/notifications';
import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { AGREEMENT_ENTITY_NAME } from '@/forms/Absence/AgreementForm/constants';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useDeleteAgreement } from '@/queries/absences/useDeleteAgreement';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export const useDeleteAgreementForm = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const {
		id: itemToDelete,
		handleOpen,
		handleClose,
		isOpen,
	} = useShowModal<SimpleConfigurableConfigurationItem<number | string>>();

	const { mutateAsync: deleteMutation, isPending: isLoading } =
		useDeleteAgreement({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						getSuccessfullyDeletedMessage(AGREEMENT_ENTITY_NAME)
					)
				);

				handleClose();

				queryClient.invalidateQueries({
					queryKey: [absencesQueriesKeys.getAbsencesData],
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
		handleOpen,
		handleClose,
		isOpen,
		handleFormSubmit,
		isLoading,
	};
};

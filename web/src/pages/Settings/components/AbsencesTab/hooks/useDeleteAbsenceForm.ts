import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useState } from 'react';

import { ABSENCE_ENTITY_NAME } from '@/constants/absence';
import {
	ABSENCE_AND_AGREEMENTS_DELETED,
	getSuccessfullyDeletedMessage,
	NotificationType,
} from '@/constants/notifications';
import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useDeleteAbsence } from '@/queries/absences/useDeleteAbsence';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export const useDeleteAbsenceForm = () => {
	const toast = useToast();
	const queryClient = useQueryClient();
	const [hasAgreements, setHasAgreements] = useState(false);

	const {
		id: itemToDelete,
		handleOpen: baseHandleOpen,
		handleClose,
		isOpen,
	} = useShowModal<SimpleConfigurableConfigurationItem<number | string>>();

	const { mutateAsync: deleteMutation, isPending: isLoading } =
		useDeleteAbsence({
			onSuccess: () => {
				const message = hasAgreements
					? ABSENCE_AND_AGREEMENTS_DELETED
					: getSuccessfullyDeletedMessage(ABSENCE_ENTITY_NAME);

				toast.open(createToast(NotificationType.SUCCESS, message));

				handleClose();

				queryClient.invalidateQueries({
					queryKey: [absencesQueriesKeys.getAbsencesData],
				});
			},
			onError: createOnErrorHandler(toast),
		});

	const handleOpen = useCallback(
		(
			item: SimpleConfigurableConfigurationItem<number | string>,
			hasConnectedAgreements: boolean = false
		) => {
			setHasAgreements(hasConnectedAgreements);
			baseHandleOpen(item);
		},
		[baseHandleOpen]
	);

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
		hasAgreements,
	};
};

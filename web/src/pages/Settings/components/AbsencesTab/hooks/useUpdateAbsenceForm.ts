import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import { ABSENCE_ENTITY_NAME } from '@/constants/absence';
import {
	getSuccessfullyUpdatedMessage,
	NotificationType,
} from '@/constants/notifications';
import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useUpdateAbsence } from '@/queries/absences/useUpdateAbsence';
import { AbsencesFormValues } from '@/types/absences';

export const useUpdateAbsenceForm = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const { mutateAsync: updateMutation, isPending: isLoading } =
		useUpdateAbsence({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						getSuccessfullyUpdatedMessage(ABSENCE_ENTITY_NAME)
					)
				);

				handleClose();

				queryClient.invalidateQueries({
					queryKey: [absencesQueriesKeys.getAbsencesData],
				});
			},
			onError: createOnErrorHandler(toast),
		});

	const handleFormSubmit = useCallback(
		async (values: AbsencesFormValues) => {
			if (id) {
				await updateMutation({
					id: id || '',
					...values,
				});
			}
		},
		[updateMutation, id]
	);

	return {
		id,
		handleOpen,
		handleClose,
		isOpen,
		handleFormSubmit,
		isLoading,
	};
};

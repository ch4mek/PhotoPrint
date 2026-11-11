import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import { ABSENCE_ENTITY_NAME } from '@/constants/absence';
import {
	getSuccessfullyAddedMessage,
	NotificationType,
} from '@/constants/notifications';
import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useCreateAbsence } from '@/queries/absences/useCreateAbsence';
import { AbsencesFormValues } from '@/types/absences';

export const useCreateAbsenceForm = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const handleCloseAbsenceForm = () => {
		handleClose();
	};

	const { mutateAsync: createMutation, isPending: isLoading } =
		useCreateAbsence({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						getSuccessfullyAddedMessage(ABSENCE_ENTITY_NAME)
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
		async (
			values: AbsencesFormValues,
			options?: {
				onSuccess?: () => void;
			}
		) => {
			await createMutation(values, options);
		},
		[createMutation]
	);

	return {
		id,
		handleOpen,
		handleClose: handleCloseAbsenceForm,
		isOpen,
		handleFormSubmit,
		isLoading,
	};
};

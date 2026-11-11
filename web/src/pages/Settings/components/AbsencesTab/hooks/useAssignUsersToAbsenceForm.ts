import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import { NotificationType } from '@/constants/notifications';
import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { SUCCESSFULLY_ASSIGNED_MESSAGE } from '@/pages/Settings/components/AbsencesTab/constants';
import { useAssignUsersToAbsence } from '@/queries/absences/useAssignUsersToAbsence';
import { AbsenceAssignmentValues } from '@/types/absences';

export const useAssignUsersToAbsenceForm = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { handleOpen, handleClose, isOpen } = useShowModal();

	const { mutateAsync: createMutation, isPending: isLoading } =
		useAssignUsersToAbsence({
			onSuccess: () => {
				toast.open(
					createToast(NotificationType.SUCCESS, SUCCESSFULLY_ASSIGNED_MESSAGE)
				);

				handleClose();

				queryClient.invalidateQueries({
					queryKey: [absencesQueriesKeys.getAbsencesData],
				});
			},
			onError: createOnErrorHandler(toast),
		});

	const handleFormSubmit = useCallback(
		async (values: AbsenceAssignmentValues) => {
			await createMutation(values);
		},
		[createMutation]
	);

	return {
		handleOpen,
		handleClose,
		isOpen,
		handleFormSubmit,
		isLoading,
	};
};

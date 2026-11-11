import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import { ASSIGNMENT_ENTITY_NAME } from '../constants';

import {
	getSuccessfullyRemovedMessage,
	NotificationType,
} from '@/constants/notifications';
import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useAssignUsersToAbsence } from '@/queries/absences/useAssignUsersToAbsence';
import { AbsenceAssignmentValues } from '@/types/absences';

export const useRemoveAssign = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { handleOpen, handleClose, isOpen } = useShowModal();

	const { mutateAsync: createMutation, isPending: isLoading } =
		useAssignUsersToAbsence({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						getSuccessfullyRemovedMessage(ASSIGNMENT_ENTITY_NAME)
					)
				);

				handleClose();

				queryClient.invalidateQueries({
					queryKey: [absencesQueriesKeys.getUserAbsences],
				});

				queryClient.invalidateQueries({
					queryKey: [absencesQueriesKeys.getUserAbsencesRequest],
				});

				queryClient.invalidateQueries({
					queryKey: [absencesQueriesKeys.getUserTransactionsHistory],
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

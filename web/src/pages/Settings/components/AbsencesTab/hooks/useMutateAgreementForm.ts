import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import {
	getSuccessfullyAddedMessage,
	getSuccessfullyUpdatedMessage,
	NotificationType,
} from '@/constants/notifications';
import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { AGREEMENT_ENTITY_NAME } from '@/forms/Absence/AgreementForm/constants';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useCreateAgreement } from '@/queries/absences/useCreateAgreement';
import { useUpdateAgreement } from '@/queries/absences/useUpdateAgreement';
import { Agreement, AgreementItem } from '@/types/absences';

export const useMutateAgreementForm = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const {
		id: agreementItem,
		handleOpen,
		handleClose,
		isOpen,
	} = useShowModal<{ itemId: string | number; item?: AgreementItem }>();

	const { mutateAsync: updateMutation, isPending: isLoadingUpdate } =
		useUpdateAgreement({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						getSuccessfullyUpdatedMessage(AGREEMENT_ENTITY_NAME)
					)
				);

				handleClose();

				queryClient.invalidateQueries({
					queryKey: [absencesQueriesKeys.getAbsencesData],
				});
			},
			onError: createOnErrorHandler(toast),
		});

	const { mutateAsync: createMutation, isPending: isLoadingCreation } =
		useCreateAgreement({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						getSuccessfullyAddedMessage(AGREEMENT_ENTITY_NAME)
					)
				);

				handleClose();

				queryClient.invalidateQueries({
					queryKey: [absencesQueriesKeys.getAbsencesData],
				});
			},
			onError: createOnErrorHandler(toast),
		});
	const handleCreateAgreementFormSubmit = useCallback(
		async (values: Agreement) => {
			await createMutation(values);
		},
		[createMutation, queryClient, toast]
	);

	const handleUpdateAgreementFormSubmit = useCallback(
		async (values: AgreementItem) => {
			if (values.id) {
				await updateMutation(values);
			}
		},
		[updateMutation, queryClient, toast]
	);

	const isLoading = isLoadingCreation || isLoadingUpdate;

	return {
		agreementItem,
		handleOpen,
		handleClose,
		isOpen,
		handleCreateAgreementFormSubmit,
		handleUpdateAgreementFormSubmit,
		isLoading,
	};
};

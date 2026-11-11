import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useCorrectAgreementDays } from '@/queries/absences/useCorrectAgreementDays';
import { AgreementCorrectionValues } from '@/types/absences';

export const useAbsenceSettings = () => {
	const queryClient = useQueryClient();

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const {
		mutateAsync: updateAgreementMutation,
		isPending: isAbsenceSettingsLoading,
	} = useCorrectAgreementDays({
		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: [absencesQueriesKeys.getUserAbsences],
			});
			queryClient.invalidateQueries({
				queryKey: [absencesQueriesKeys.getUserTransactionsHistory],
			});

			handleClose();
		},
	});

	const handleAbsenceSettingsSubmit = useCallback(
		async (values: Omit<AgreementCorrectionValues, 'agreementId'>) => {
			await updateAgreementMutation({
				...values,
				agreementId: id,
			} as AgreementCorrectionValues);
		},
		[updateAgreementMutation, id]
	);

	return {
		AbsenceSettingsFormId: id,
		handleAbsenceSettingsFormOpen: handleOpen,
		handleAbsenceSettingsFormClose: handleClose,
		isAbsenceSettingsFormOpen: isOpen,
		handleAbsenceSettingsSubmit,
		isAbsenceSettingsLoading,
	};
};

import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ABSENCE_ENTITY_NAME } from '@/constants/absence';
import {
	NotificationType,
	getSuccessfullyUpdatedMessage,
} from '@/constants/notifications';
import { getAgreementCorrectionByIdEndpoint } from '@/constants/queryPaths';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import {
	AgreementCorrectionValues,
	AgreementCorrectionResponse,
} from '@/types/absences';

export const useCorrectAgreementDays = (
	options: UseMutationOptions<
		AgreementCorrectionResponse,
		AxiosError,
		AgreementCorrectionValues
	>
) => {
	const toast = useToast();

	return useMutation<
		AgreementCorrectionResponse,
		AxiosError,
		AgreementCorrectionValues
	>({
		...options,
		mutationFn: ({ userId, agreementId, ...data }) =>
			axios
				.post(getAgreementCorrectionByIdEndpoint(userId, agreementId), data)
				.then((res) => {
					toast.open(
						createToast(
							NotificationType.SUCCESS,
							getSuccessfullyUpdatedMessage(ABSENCE_ENTITY_NAME)
						)
					);

					return res.data;
				})
				.catch((error) => {
					if (error?.response?.data?.Message) {
						toast.open(
							createToast(NotificationType.ERROR, error.response.data.Message)
						);
					}
				}),
	});
};

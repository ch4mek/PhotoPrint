import { useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import { MERCH_KITS_FIELD_TITLE } from '../constants';

import {
	getSuccessfullyAddedMessage,
	getSuccessfullyUpdatedMessage,
	MAXIMUM_ALLOWED_CLOTHING_KITS_MESSAGE,
	MAXIMUM_ALLOWED_CLOTHING_KITS_TITLE,
	NotificationType,
} from '@/constants/notifications';
import { merchQueriesKeys } from '@/constants/queriesKeys';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { ToastContextValues } from '@/context/types';
import { TRANSACTION_FIELD_TITLE } from '@/forms/MerchKitsTransactionForm/constants';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useAddKitTransaction } from '@/queries/merchKits/useAddKitTransaction';
import { useAddMerchKit } from '@/queries/merchKits/useAddMerchKit';
import { useUpdateMerchKit } from '@/queries/merchKits/useUpdateMerchKit';
import { ServerResponseError } from '@/queries/types';

const createKitErrorHandler =
	(toast: ToastContextValues) =>
	(error: AxiosError<ServerResponseError, unknown>) => {
		const errorMessage =
			error.response?.data?.Message || error.response?.data?.message || '';

		if (errorMessage.includes(MAXIMUM_ALLOWED_CLOTHING_KITS_MESSAGE)) {
			createOnErrorHandler(toast, MAXIMUM_ALLOWED_CLOTHING_KITS_TITLE)(error);
		} else {
			createOnErrorHandler(toast)(error);
		}
	};

export const useKitsTabContext = (
	handleClose: VoidFunction,
	handleTransactionClose: VoidFunction
) => {
	const queryClient = useQueryClient();
	const toast = useToast();
	const {
		configurableValues: { merchStorages, merchSizes },
	} = useConfiguration();

	const { mutateAsync: addMerchKit } = useAddMerchKit({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyAddedMessage(MERCH_KITS_FIELD_TITLE)
				)
			);
			handleClose();
			queryClient.invalidateQueries({
				queryKey: [merchQueriesKeys.getMerchKitsData],
			});
		},
		onError: createKitErrorHandler(toast),
	});

	const { mutateAsync: updateMerchKit } = useUpdateMerchKit({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyUpdatedMessage(MERCH_KITS_FIELD_TITLE)
				)
			);

			handleClose();
			queryClient.invalidateQueries({
				queryKey: [merchQueriesKeys.getMerchKitsData],
			});
		},
		onError: createOnErrorHandler(toast),
	});

	const { mutateAsync: addKitTransaction } = useAddKitTransaction({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyAddedMessage(TRANSACTION_FIELD_TITLE)
				)
			);
			handleTransactionClose();
			queryClient.invalidateQueries({
				queryKey: [merchQueriesKeys.getMerchKitsData],
			});
		},
		onError: createOnErrorHandler(toast),
	});

	return {
		addMerchKit,
		updateMerchKit,
		addKitTransaction,
		merchSizes,
		merchStorages,
	};
};

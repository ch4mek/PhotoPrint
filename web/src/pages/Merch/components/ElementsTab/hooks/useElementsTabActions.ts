import { useQueryClient } from '@tanstack/react-query';

import {
	getSuccessfullyAddedMessage,
	getSuccessfullyUpdatedMessage,
	NotificationType,
} from '@/constants/notifications';
import { merchQueriesKeys } from '@/constants/queriesKeys';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { MERCH_ELEMENTS_FIELD_TITLE } from '@/forms/MerchElementForm/constants';
import { MerchElementUpdateStatus } from '@/forms/MerchElementForm/types';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useAddMerchElement } from '@/queries/merchElements/useAddMerchElement';
import { useUpdateMerchElement } from '@/queries/merchElements/useUpdateMerchElement';
import { useUpdateMerchElementStatus } from '@/queries/merchElements/useUpdateMerchElementStatus';
import { UserClaims } from '@/types/claims';

export const useElementsTabContext = (
	handleClose: VoidFunction,
	handleOpenTransactionForm: VoidFunction,
	merchId?: number | string
) => {
	const queryClient = useQueryClient();
	const toast = useToast();
	const { claims } = useAuth();
	const {
		configurableValues: { merchStorages, merchSizes },
	} = useConfiguration();

	const {
		mutateAsync: addMerchElement,
		isPending: isAddingMerchElement,
		data: addedMerchElement,
	} = useAddMerchElement({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyAddedMessage(MERCH_ELEMENTS_FIELD_TITLE)
				)
			);
			handleClose();

			if (checkIsHasClaims([UserClaims.ManageMerchTransactions], claims)) {
				handleOpenTransactionForm();
			}
			queryClient.invalidateQueries({
				queryKey: [merchQueriesKeys.getMerchElementsData],
			});
			queryClient.invalidateQueries({
				queryKey: [merchQueriesKeys.merchElementsLookup],
			});
		},
		onError: createOnErrorHandler(toast),
	});

	const { mutateAsync: updateMerchElement, isPending: isUpdatingMerchElement } =
		useUpdateMerchElement({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						getSuccessfullyUpdatedMessage(MERCH_ELEMENTS_FIELD_TITLE)
					)
				);

				handleClose();
				queryClient.invalidateQueries({
					queryKey: [merchQueriesKeys.getMerchElementsData],
				});
				queryClient.invalidateQueries({
					queryKey: [merchQueriesKeys.merchElementsLookup],
				});
			},
			onError: createOnErrorHandler(toast),
		});

	const {
		mutateAsync: updateMerchElementStatus,
		isPending: isUpdatingMerchElementStatus,
	} = useUpdateMerchElementStatus(merchId, {
		onError: createOnErrorHandler(toast),
	});

	const handleUpdateStatus = async (
		values: MerchElementUpdateStatus,
		successMessage: string
	) =>
		updateMerchElementStatus(values, {
			onSuccess: () => {
				toast.open(createToast(NotificationType.SUCCESS, successMessage));

				handleClose();
				queryClient.invalidateQueries({
					queryKey: [merchQueriesKeys.getMerchElementsData],
				});
			},
		});

	return {
		addMerchElement,
		updateMerchElement,
		updateMerchElementStatus: handleUpdateStatus,
		merchSizes,
		merchStorages,
		addedMerchElement,
		isAddingMerchElement,
		isUpdatingMerchElement,
		isUpdatingMerchElementStatus,
	};
};

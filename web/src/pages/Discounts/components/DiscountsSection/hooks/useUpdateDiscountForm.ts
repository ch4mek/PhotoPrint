import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import {
	getSuccessfullyUpdatedMessage,
	NotificationType,
} from '@/constants/notifications';
import { discountQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useUpdateDiscount } from '@/queries/discounts/useUpdateDiscount';
import {
	DiscountSegmentTypes,
	DiscountSubmitData,
	DiscountUpdateData,
} from '@/types/discounts';

export const useUpdateDiscountForm = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const {
		mutateAsync: updateDiscountMutation,
		isPending: isUpdateDiscountLoading,
	} = useUpdateDiscount({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyUpdatedMessage(DiscountSegmentTypes.Discount)
				)
			);

			queryClient.invalidateQueries({
				queryKey: [discountQueriesKeys.getDiscountsData],
			});
			queryClient.invalidateQueries({
				queryKey: [discountQueriesKeys.getDiscountByIdData, id],
			});

			handleClose();
		},
		onError: createOnErrorHandler(toast),
	});

	const handleUpdateDiscountFormSubmit = useCallback(
		async (values: DiscountSubmitData) => {
			await updateDiscountMutation({ ...values, id } as DiscountUpdateData);
		},
		[updateDiscountMutation, id]
	);

	return {
		updateDiscountFormId: id,
		handleUpdateDiscountFormOpen: handleOpen,
		handleUpdateDiscountFormClose: handleClose,
		isUpdateDiscountFormOpen: isOpen,
		handleUpdateDiscountFormSubmit,
		isUpdateDiscountLoading,
	};
};

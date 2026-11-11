import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import {
	getSuccessfullyDeletedMessage,
	NotificationType,
} from '@/constants/notifications';
import { discountQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { DISCOUNT_TITLE } from '@/pages/Discounts/components/DiscountsSection/constants';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useDeleteDiscount } from '@/queries/discounts/useDeleteDiscount';

export const useDeleteDiscountForm = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const {
		mutateAsync: deleteDiscountMutation,
		isPending: isDeleteDiscountLoading,
	} = useDeleteDiscount({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyDeletedMessage(DISCOUNT_TITLE)
				)
			);

			handleClose();

			queryClient.invalidateQueries({
				queryKey: [discountQueriesKeys.getDiscountsData],
			});
		},
		onError: createOnErrorHandler(toast),
	});

	const handleDeleteDiscountSubmit = useCallback(async () => {
		if (id != null) {
			await deleteDiscountMutation(id);
		}
	}, [id]);

	return {
		deleteDiscountFormId: id,
		handleDeleteDiscountFormOpen: handleOpen,
		handleDeleteDiscountFormClose: handleClose,
		isDeleteDiscountFormOpen: isOpen,
		isDeleteDiscountLoading,
		handleDeleteDiscountSubmit,
	};
};

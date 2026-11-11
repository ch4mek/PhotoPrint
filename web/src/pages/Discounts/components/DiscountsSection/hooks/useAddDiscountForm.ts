import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import {
	getSuccessfullyAddedMessage,
	NotificationType,
} from '@/constants/notifications';
import { discountQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useCreateDiscount } from '@/queries/discounts/useCreateDiscount';
import {
	DiscountAddData,
	DiscountSegmentTypes,
	DiscountSubmitData,
} from '@/types/discounts';

export const useAddDiscountForm = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const {
		mutateAsync: createDiscountMutation,
		isPending: isCreateDiscountLoading,
	} = useCreateDiscount({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyAddedMessage(DiscountSegmentTypes.Discount)
				)
			);

			handleClose();

			queryClient.invalidateQueries({
				queryKey: [discountQueriesKeys.getDiscountsData],
			});
		},
		onError: createOnErrorHandler(toast),
	});

	const handleAddDiscountFormSubmit = useCallback(
		async (values: DiscountSubmitData) => {
			await createDiscountMutation(values as DiscountAddData);
		},
		[createDiscountMutation]
	);

	return {
		createDiscountFormId: id,
		handleCreateDiscountFormOpen: handleOpen,
		handleCreateDiscountFormClose: handleClose,
		isCreateDiscountFormOpen: isOpen,
		handleAddDiscountFormSubmit,
		isCreateDiscountLoading,
	};
};

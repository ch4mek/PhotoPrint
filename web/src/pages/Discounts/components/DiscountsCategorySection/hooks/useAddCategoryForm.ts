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
import { SECTION_FIELD_TITLE } from '@/pages/Discounts/components/DiscountsCategorySection/constants';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useCreateDiscountCategory } from '@/queries/discounts/categories/useCreateDiscountCategory';
import { DiscountCategoryMutationData } from '@/types/discountCategories';

export const useAddCategoryForm = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const {
		mutateAsync: createCategoryMutation,
		isPending: isCreateCategoryLoading,
	} = useCreateDiscountCategory({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyAddedMessage(SECTION_FIELD_TITLE)
				)
			);

			handleClose();

			queryClient.invalidateQueries({
				queryKey: [discountQueriesKeys.getDiscountCategoriesData],
			});
		},
		onError: createOnErrorHandler(toast),
	});

	const handleAddCategoryFormSubmit = useCallback(
		async (values: DiscountCategoryMutationData) => {
			await createCategoryMutation(values);
		},
		[createCategoryMutation]
	);

	return {
		createCategoryFormId: id,
		handleCreateCategoryFormOpen: handleOpen,
		handleCreateCategoryFormClose: handleClose,
		isCreateCategoryFormOpen: isOpen,
		handleAddCategoryFormSubmit,
		isCreateCategoryLoading,
	};
};

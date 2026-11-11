import { useQueryClient } from '@tanstack/react-query';

import {
	getSuccessfullyDeletedMessage,
	NotificationType,
} from '@/constants/notifications';
import { discountQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { SECTION_FIELD_TITLE } from '@/pages/Discounts/components/DiscountsCategorySection/constants';
import { UseDeleteCategoryForm } from '@/pages/Discounts/components/DiscountsCategorySection/types';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useDeleteDiscountCategory } from '@/queries/discounts/categories/useDeleteDiscountCategory';

export const useDeleteCategoryForm = ({
	setActiveCategoryId,
	setActiveTagId,
	activeCategoryId,
}: UseDeleteCategoryForm) => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const handleUpdateDataAfterDeleting = () => {
		setActiveCategoryId(null);
		setActiveTagId(null);
	};

	const {
		mutateAsync: deleteCategoryMutation,
		isPending: isDeleteCategoryLoading,
	} = useDeleteDiscountCategory({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyDeletedMessage(SECTION_FIELD_TITLE)
				)
			);

			handleClose();

			handleUpdateDataAfterDeleting();

			queryClient.invalidateQueries({
				queryKey: [discountQueriesKeys.getDiscountCategoriesData],
			});

			if (activeCategoryId && activeCategoryId === id) {
				handleUpdateDataAfterDeleting();
			}
		},
		onError: createOnErrorHandler(toast),
	});

	return {
		deleteCategoryFormId: id,
		handleDeleteCategoryFormOpen: handleOpen,
		handleDeleteCategoryFormClose: handleClose,
		isDeleteCategoryFormOpen: isOpen,
		isDeleteCategoryLoading,
		deleteCategoryMutation,
	};
};

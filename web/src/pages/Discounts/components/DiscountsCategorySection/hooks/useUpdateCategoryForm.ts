import { useQueryClient } from '@tanstack/react-query';

import {
	getSuccessfullyUpdatedMessage,
	NotificationType,
} from '@/constants/notifications';
import { discountQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { SECTION_FIELD_TITLE } from '@/pages/Discounts/components/DiscountsCategorySection/constants';
import { UseUpdateCategoryFormProps } from '@/pages/Discounts/components/DiscountsCategorySection/types';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useUpdateDiscountCategory } from '@/queries/discounts/categories/useUpdateDiscountCategory';

export const useUpdateCategoryForm = ({
	activeCategoryId,
}: UseUpdateCategoryFormProps) => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const {
		mutateAsync: updateCategoryMutation,
		isPending: isUpdateCategoryLoading,
	} = useUpdateDiscountCategory({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyUpdatedMessage(SECTION_FIELD_TITLE)
				)
			);

			handleClose();

			queryClient.invalidateQueries({
				queryKey: [discountQueriesKeys.getDiscountCategoriesData],
			});

			if (activeCategoryId && activeCategoryId === id) {
				queryClient.invalidateQueries({
					queryKey: [discountQueriesKeys.getDiscountsTagsByCategoryIdData],
				});
			}
		},
		onError: createOnErrorHandler(toast),
	});

	return {
		updateCategoryFormId: id,
		handleUpdateCategoryFormOpen: handleOpen,
		handleUpdateCategoryFormClose: handleClose,
		isUpdateCategoryFormOpen: isOpen,
		updateCategoryMutation,
		isUpdateCategoryLoading,
	};
};

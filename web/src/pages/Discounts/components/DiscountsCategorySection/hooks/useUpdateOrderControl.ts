import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import {
	getSuccessfullyUpdatedMessage,
	NotificationType,
} from '@/constants/notifications';
import { discountQueriesKeys } from '@/constants/queriesKeys';
import { useDiscountContext } from '@/context/DiscountContext';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { getUpdateOrderTitle } from '@/pages/Discounts/components/DiscountsCategorySection/constants';
import { useUpdateDiscountCategoryOrder } from '@/queries/discounts/categories/useUpdateDiscountCategoryOrder';
import { useUpdateDiscountOrder } from '@/queries/discounts/useUpdateDiscountOrder';
import { DiscountSegmentTypes } from '@/types/discounts';

const { getDiscountsData, getDiscountCategoriesData } = discountQueriesKeys;

export function useUpdateOrderControl() {
	const toast = useToast();
	const queryClient = useQueryClient();

	const {
		setIsMoveModeEnable,
		setDiscountOrderData,
		setCategoryOrderData,
		categoryOrderData,
		discountOrderData,
		isMoveModeEnable,
	} = useDiscountContext();

	const getSuccessOrderToast = (title: string) => {
		toast.open(
			createToast(
				NotificationType.SUCCESS,
				getSuccessfullyUpdatedMessage(getUpdateOrderTitle(title.toLowerCase()))
			)
		);
	};

	const { mutateAsync: mutateDiscountCategoryOrder } =
		useUpdateDiscountCategoryOrder({
			onSuccess: () => {
				setCategoryOrderData([]);
				queryClient.invalidateQueries({
					queryKey: [getDiscountCategoriesData],
				});
				setIsMoveModeEnable(false);
				getSuccessOrderToast(DiscountSegmentTypes.Category);
			},
			onError: createOnErrorHandler(toast),
		});

	const { mutateAsync: mutateDiscountOrder } = useUpdateDiscountOrder({
		onSuccess: () => {
			setDiscountOrderData([]);
			queryClient.invalidateQueries({ queryKey: [getDiscountsData] });
			setIsMoveModeEnable(false);
			getSuccessOrderToast(DiscountSegmentTypes.Discount);
		},
		onError: createOnErrorHandler(toast),
	});

	const handleModeButtonClick = useCallback(async () => {
		if (
			!isMoveModeEnable ||
			(!categoryOrderData.length && !discountOrderData.length)
		) {
			setIsMoveModeEnable((prevState) => !prevState);

			return;
		}

		if (categoryOrderData.length) {
			await mutateDiscountCategoryOrder({
				discountCategoryOrders: categoryOrderData,
			});
		}

		if (discountOrderData.length) {
			await mutateDiscountOrder({
				discountOrders: discountOrderData,
			});
		}
	}, [categoryOrderData, discountOrderData, isMoveModeEnable]);

	const handleCancelButtonClick = useCallback(() => {
		setIsMoveModeEnable(false);
		setCategoryOrderData([]);
		setDiscountOrderData([]);
	}, [setIsMoveModeEnable, setCategoryOrderData]);

	return {
		handleModeButtonClick,
		handleCancelButtonClick,
	};
}

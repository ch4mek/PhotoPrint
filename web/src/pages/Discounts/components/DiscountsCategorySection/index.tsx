import { useMemo } from 'react';

import { Loader } from '@/components/Loader';
import { useAuth } from '@/context/AuthContext';
import { useDiscountContext } from '@/context/DiscountContext';
import { DiscountCategoryForm } from '@/forms/Discount/DiscountCategoryForm';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { CategoryControls } from '@/pages/Discounts/components/DiscountsCategorySection/components/CategoryControls';
import { CategoryList } from '@/pages/Discounts/components/DiscountsCategorySection/components/CategoryList';
import { useAddCategoryForm } from '@/pages/Discounts/components/DiscountsCategorySection/hooks/useAddCategoryForm';
import { useUpdateOrderControl } from '@/pages/Discounts/components/DiscountsCategorySection/hooks/useUpdateOrderControl';
import { DiscountsCategorySectionWrapper } from '@/pages/Discounts/components/DiscountsCategorySection/styles';
import { getSortedDiscountCategories } from '@/pages/Discounts/components/DiscountsCategorySection/utils/getSortedDiscountCategories';
import { useGetDiscountCategories } from '@/queries/discounts/categories/useGetDiscountCategories';
import { UserClaims } from '@/types/claims';

export function DiscountsCategorySection() {
	const { claims } = useAuth();
	const { isMoveModeEnable, isMoveEnable } = useDiscountContext();
	const {
		data: discountCategories,
		isFetching,
		isLoading,
	} = useGetDiscountCategories();

	const sortedDiscountCategories = useMemo(
		() => getSortedDiscountCategories(discountCategories),
		[discountCategories]
	);

	const {
		isCreateCategoryFormOpen,
		handleCreateCategoryFormOpen,
		handleCreateCategoryFormClose,
		handleAddCategoryFormSubmit,
	} = useAddCategoryForm();

	const { handleCancelButtonClick, handleModeButtonClick } =
		useUpdateOrderControl();

	const isShowLoader = isFetching || isLoading;

	return (
		<DiscountsCategorySectionWrapper>
			{checkIsHasClaims(
				[UserClaims.ManageDiscountCategory, UserClaims.ManageDiscountOrder],
				claims
			) && (
				<CategoryControls
					handleAddButton={handleCreateCategoryFormOpen}
					handleCancelButtonClick={handleCancelButtonClick}
					handleModeButtonClick={handleModeButtonClick}
					isButtonsDisabled={isShowLoader}
					isMoveModeEnable={isMoveModeEnable}
					isMoveEnable={isMoveEnable}
				/>
			)}
			<Loader isLoading={isShowLoader}>
				<CategoryList
					data={sortedDiscountCategories}
					isMoveMode={isMoveModeEnable}
				/>
			</Loader>

			{isCreateCategoryFormOpen && (
				<DiscountCategoryForm
					handleSubmit={handleAddCategoryFormSubmit}
					handleClose={handleCreateCategoryFormClose}
				/>
			)}
		</DiscountsCategorySectionWrapper>
	);
}

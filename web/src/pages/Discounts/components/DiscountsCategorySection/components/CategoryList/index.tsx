import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { useCallback, useMemo, useState } from 'react';

import { MenuItem } from '@/components/DraggableControlIcon/types';
import { useDiscountContext } from '@/context/DiscountContext';
import { DiscountCategoryForm } from '@/forms/Discount/DiscountCategoryForm';
import { DeleteConfirmationDialog } from '@/pages/Discounts/components/DeleteConfirmationDialog';
import { CategoryItem } from '@/pages/Discounts/components/DiscountsCategorySection/components/CategoryItem';
import { CategoryListWrapper } from '@/pages/Discounts/components/DiscountsCategorySection/components/CategoryList/styles';
import { CategoryListProps } from '@/pages/Discounts/components/DiscountsCategorySection/components/CategoryList/types';
import {
	ALL_CATEGORIES_TITLE,
	allCategoriesItem,
} from '@/pages/Discounts/components/DiscountsCategorySection/constants';
import { useDeleteCategoryForm } from '@/pages/Discounts/components/DiscountsCategorySection/hooks/useDeleteCategoryForm';
import { useUpdateCategoryForm } from '@/pages/Discounts/components/DiscountsCategorySection/hooks/useUpdateCategoryForm';
import { DraggableItem } from '@/pages/Discounts/components/DraggableItem';
import {
	DiscountCategoryMutationData,
	DiscountCategoryOrder,
} from '@/types/discountCategories';
import { DiscountSegmentTypes } from '@/types/discounts';

import { getCategoryListFromOrderData } from './utils';

export function CategoryList({ data, isMoveMode }: CategoryListProps) {
	const {
		activeCategoryId,
		categoryOrderData,
		setActiveCategoryId,
		setActiveTagId,
		setCategoryOrderData,
	} = useDiscountContext();
	const [deletedTitle, setDeletedTitle] = useState('');

	const {
		updateCategoryFormId,
		handleUpdateCategoryFormOpen,
		handleUpdateCategoryFormClose,
		isUpdateCategoryFormOpen,
		updateCategoryMutation,
	} = useUpdateCategoryForm({
		activeCategoryId,
	});

	const {
		deleteCategoryFormId,
		handleDeleteCategoryFormOpen,
		handleDeleteCategoryFormClose,
		isDeleteCategoryFormOpen,
		isDeleteCategoryLoading,
		deleteCategoryMutation,
	} = useDeleteCategoryForm({
		setActiveCategoryId,
		setActiveTagId,
		activeCategoryId,
	});

	const handleDeleteCategoryClick = (
		categoryId: string | number,
		title: string
	) => {
		setDeletedTitle(title);
		handleDeleteCategoryFormOpen(categoryId);
	};

	const itemMenuOption = useMemo(
		() => [
			{
				value: 'Edit',
				addon: <EditIcon />,
				onClick: ({ id }: MenuItem) => handleUpdateCategoryFormOpen(id),
			},
			{
				value: 'Delete',
				addon: <BasketIcon />,
				onClick: ({ id, name }: MenuItem) =>
					handleDeleteCategoryClick(id, name),
			},
		],
		[]
	);

	const handleClick = useCallback(
		(categoryId: number | string) => () => {
			if (isMoveMode) {
				return;
			}
			const selectedId =
				categoryId === ALL_CATEGORIES_TITLE || typeof categoryId === 'string'
					? null
					: categoryId;
			setActiveCategoryId(selectedId);
			setActiveTagId(null);
		},
		[isMoveMode]
	);

	const handleUpdateSubmit = useCallback(
		async (values: DiscountCategoryMutationData) => {
			if (updateCategoryFormId == null) {
				return;
			}

			await updateCategoryMutation({
				id: updateCategoryFormId,
				...values,
			});
		},
		[updateCategoryMutation, updateCategoryFormId]
	);

	const handleDeleteCategorySubmit = useCallback(async () => {
		if (deleteCategoryFormId == null) {
			return;
		}

		await deleteCategoryMutation(deleteCategoryFormId);
	}, [deleteCategoryMutation, deleteCategoryFormId]);

	const categoryList = useMemo(
		getCategoryListFromOrderData(data, categoryOrderData),
		[categoryOrderData]
	);

	const moveCard = useCallback(
		(dragIndex: number, hoverIndex: number) => {
			const updatedData = [...(categoryList || [])];
			const [removed] = updatedData.splice(dragIndex, 1);
			updatedData.splice(hoverIndex, 0, removed);

			setCategoryOrderData(
				updatedData.map(
					(item, index) =>
						({
							discountCategoryId: item.id,
							orderWeight: index,
						} as DiscountCategoryOrder)
				)
			);
		},
		[categoryList]
	);

	const displayedCategoriesOrder = useMemo(
		() => (categoryOrderData.length ? categoryList : data),
		[categoryOrderData, categoryList, data]
	);

	return (
		<CategoryListWrapper>
			<CategoryItem
				item={allCategoriesItem}
				isReadMode
				handleClick={handleClick}
				isSelectedItem={activeCategoryId === null}
			/>
			{displayedCategoriesOrder?.map((item, index) => (
				<DraggableItem
					key={item.id}
					index={index}
					id={item.id}
					type={DiscountSegmentTypes.Category}
					moveCard={moveCard}
					isDragMode={isMoveMode}
				>
					<CategoryItem
						item={item}
						isMoveMode={isMoveMode}
						menuOptions={itemMenuOption}
						handleClick={handleClick}
						isSelectedItem={activeCategoryId === item.id}
					/>
				</DraggableItem>
			))}

			{updateCategoryFormId && isUpdateCategoryFormOpen && (
				<DiscountCategoryForm
					categoryId={updateCategoryFormId}
					handleSubmit={handleUpdateSubmit}
					handleClose={handleUpdateCategoryFormClose}
				/>
			)}

			{isDeleteCategoryFormOpen && deleteCategoryFormId && (
				<DeleteConfirmationDialog
					handleCloseDialog={handleDeleteCategoryFormClose}
					handleDeleteSegment={handleDeleteCategorySubmit}
					showLoader={isDeleteCategoryLoading}
					segmentType={DiscountSegmentTypes.Category}
					title={deletedTitle}
				/>
			)}
		</CategoryListWrapper>
	);
}

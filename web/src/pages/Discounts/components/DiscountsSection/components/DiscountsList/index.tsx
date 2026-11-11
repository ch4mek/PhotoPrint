import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

import { DiscountItemDragLayer } from '../DiscountItemDragLayer';

import { NoDataFoundWrapper } from '@/components/NoDataFoundWrapper';
import { discountQueriesKeys } from '@/constants/queriesKeys';
import { Routes } from '@/constants/routes';
import { useDiscountContext } from '@/context/DiscountContext';
import { DiscountForm } from '@/forms/Discount/DiscountForm';
import { DiscountViewForm } from '@/forms/Discount/DiscountViewForm';
import { getIdFromPathname } from '@/helpers/getIdFromPathname';
import { DeleteConfirmationDialog } from '@/pages/Discounts/components/DeleteConfirmationDialog';
import { DiscountItem } from '@/pages/Discounts/components/DiscountsSection/components/DiscountItem';
import {
	DiscountsCardsWrapper,
	DiscountsListWrapper,
} from '@/pages/Discounts/components/DiscountsSection/components/DiscountsList/styles';
import { DiscountsListProps } from '@/pages/Discounts/components/DiscountsSection/components/DiscountsList/types';
import { DISCOUNT_NOT_FOUND_TITLE } from '@/pages/Discounts/components/DiscountsSection/constants';
import { useDeleteDiscountForm } from '@/pages/Discounts/components/DiscountsSection/hooks/useDeleteDiscountForm';
import { useUpdateDiscountForm } from '@/pages/Discounts/components/DiscountsSection/hooks/useUpdateDiscountForm';
import { DraggableItem } from '@/pages/Discounts/components/DraggableItem';
import { useShowModal } from '@/pages/hooks/useShowModal';
import {
	Discount,
	DiscountOrder,
	DiscountSegmentTypes,
} from '@/types/discounts';

import { getDiscountsListFromOrderData } from './utils';

export function DiscountsList({ data }: DiscountsListProps) {
	const navigate = useNavigate();
	const location = useLocation();
	const queryClient = useQueryClient();
	const [deletedTitle, setDeletedTitle] = useState('');

	const { isMoveModeEnable, discountOrderData, setDiscountOrderData } =
		useDiscountContext();
	const {
		id: vewFormId,
		handleOpen: handleOpenView,
		handleClose: handleCloseView,
		isOpen: isViewOpen,
	} = useShowModal();

	const {
		handleDeleteDiscountFormOpen,
		handleDeleteDiscountFormClose,
		isDeleteDiscountFormOpen,
		isDeleteDiscountLoading,
		handleDeleteDiscountSubmit,
	} = useDeleteDiscountForm();

	const {
		updateDiscountFormId,
		handleUpdateDiscountFormOpen,
		handleUpdateDiscountFormClose,
		isUpdateDiscountFormOpen,
		handleUpdateDiscountFormSubmit,
	} = useUpdateDiscountForm();

	const handleDeleteClick = useCallback(
		(id: string | number, title: string) => {
			handleDeleteDiscountFormOpen(id);
			setDeletedTitle(title);
		},
		[handleDeleteDiscountFormOpen]
	);

	const handleEditClick = useCallback(
		(id: string | number) => {
			handleUpdateDiscountFormOpen(id);
		},
		[handleUpdateDiscountFormOpen]
	);

	const handleItemClick = useCallback(
		(id: string | number) => {
			navigate(`${Routes.discounts}/${id}`);
			handleOpenView(id);
		},
		[navigate, handleOpenView]
	);

	const handleCloseViewForm = useCallback(() => {
		navigate({ pathname: Routes.discounts, search: location.search });

		queryClient.removeQueries({
			queryKey: [discountQueriesKeys.getDiscountByIdData],
		});

		handleCloseView();
	}, [queryClient, location.search, navigate, handleCloseView]);

	const discountList = useMemo(
		getDiscountsListFromOrderData(data, discountOrderData),
		[data, discountOrderData]
	);

	const moveCard = useCallback(
		(dragIndex: number, hoverIndex: number) => {
			const updatedData: Discount[] = [...(discountList || [])];
			const [removed] = updatedData.splice(dragIndex, 1);
			updatedData.splice(hoverIndex, 0, removed);

			const updateDiscountOrder = updatedData.map(
				(item, index) =>
					({
						discountId: Number(item.id),
						orderWeight: index,
					} as DiscountOrder)
			);

			setDiscountOrderData(updateDiscountOrder);
		},
		[discountList]
	);

	const displayedDiscountsList = useMemo(
		() => (discountOrderData.length ? discountList : data),
		[discountOrderData, discountList, data]
	);

	useEffect(() => {
		if (location.pathname !== Routes.discounts) {
			const discountId = getIdFromPathname(location.pathname);
			handleOpenView(discountId);
		}
	}, [location.pathname, handleOpenView]);

	return (
		<DiscountsListWrapper>
			<NoDataFoundWrapper
				title={DISCOUNT_NOT_FOUND_TITLE}
				isEmptyData={!data || data.length === 0}
			>
				<DiscountsCardsWrapper>
					{displayedDiscountsList?.map((item, index) => (
						<DraggableItem
							key={item.id}
							id={item.id}
							index={index}
							moveCard={moveCard}
							isDragMode={isMoveModeEnable}
							type={DiscountSegmentTypes.Discount}
						>
							<DiscountItem
								key={item.id}
								item={item}
								isMoveMode={isMoveModeEnable}
								handleDeleteClick={handleDeleteClick}
								handleEditClick={handleEditClick}
								handleItemClick={handleItemClick}
							/>
						</DraggableItem>
					))}
				</DiscountsCardsWrapper>
			</NoDataFoundWrapper>

			<DiscountItemDragLayer />

			{isDeleteDiscountFormOpen && (
				<DeleteConfirmationDialog
					handleCloseDialog={handleDeleteDiscountFormClose}
					showLoader={isDeleteDiscountLoading}
					segmentType={DiscountSegmentTypes.Discount}
					handleDeleteSegment={handleDeleteDiscountSubmit}
					title={deletedTitle}
				/>
			)}

			{isUpdateDiscountFormOpen && (
				<DiscountForm
					handleClose={handleUpdateDiscountFormClose}
					handleSubmit={handleUpdateDiscountFormSubmit}
					discountId={updateDiscountFormId}
				/>
			)}

			{isViewOpen && (
				<DiscountViewForm
					discountId={vewFormId}
					handleClose={handleCloseViewForm}
				/>
			)}
		</DiscountsListWrapper>
	);
}

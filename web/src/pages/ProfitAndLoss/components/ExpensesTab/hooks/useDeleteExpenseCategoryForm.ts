import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import { ExpensesGridRow } from '../types';

import {
	getHasBeenSuccessfullyDeletedMessage,
	NotificationType,
} from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { EXPENSE_CATEGORY_ENTITY_NAME } from '@/forms/ExpenseForm/constants';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useDeleteExpenseCategory } from '@/queries/expenses/useDeleteExpenseCategory';

import { invalidateExpenseQueries } from './utils/invalidateExpenseQueries';

export const useDeleteExpenseCategoryForm = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const {
		isOpen: isDeleteDialogOpen,
		handleOpen: handleOpenDeleteDialog,
		handleClose: handleCloseDeleteDialog,
		id: expenseToDelete,
	} = useShowModal<ExpensesGridRow>();

	const {
		mutateAsync: deleteExpenseCategory,
		isPending: isDeleteExpenseCategoryLoading,
	} = useDeleteExpenseCategory({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getHasBeenSuccessfullyDeletedMessage(EXPENSE_CATEGORY_ENTITY_NAME)
				)
			);
			handleCloseDeleteDialog();
			invalidateExpenseQueries(queryClient);
		},
		onError: createOnErrorHandler(toast),
	});

	const handleDelete = useCallback(
		(row: ExpensesGridRow) => {
			handleOpenDeleteDialog(row);
		},
		[handleOpenDeleteDialog]
	);

	const handleSubmitDeleteDialog = useCallback(async () => {
		if (!expenseToDelete) {
			return;
		}

		await deleteExpenseCategory(expenseToDelete.id);
	}, [deleteExpenseCategory, expenseToDelete]);

	return {
		isDeleteDialogOpen,
		expenseToDelete,
		handleCloseDeleteDialog,
		handleSubmitDeleteDialog,
		isDeleteExpenseCategoryLoading,
		handleDelete,
	};
};

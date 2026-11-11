import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useMemo } from 'react';

import { ExpensesGridRow } from '../types';

import {
	getHasBeenSuccessfullyAddedMessage,
	getHasBeenSuccessfullyUpdatedMessage,
	NotificationType,
} from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { EXPENSE_CATEGORY_ENTITY_NAME } from '@/forms/ExpenseForm/constants';
import { ExpenseFormValues } from '@/forms/ExpenseForm/types';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useCreateExpenseCategory } from '@/queries/expenses/useCreateExpenseCategory';
import { useUpdateExpenseCategory } from '@/queries/expenses/useUpdateExpenseCategory';

import { invalidateExpenseQueries } from './utils/invalidateExpenseQueries';

export const useExpenseCategoryForm = ({
	rows,
}: {
	rows: ExpensesGridRow[];
}) => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const {
		isOpen: isExpenseFormOpen,
		id: expenseId,
		handleOpen: handleOpenExpenseForm,
		handleClose: handleCloseExpenseForm,
	} = useShowModal<number>();

	const { mutateAsync: createExpenseCategory } = useCreateExpenseCategory({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getHasBeenSuccessfullyAddedMessage(EXPENSE_CATEGORY_ENTITY_NAME)
				)
			);
			handleCloseExpenseForm();
			invalidateExpenseQueries(queryClient);
		},
		onError: createOnErrorHandler(toast),
	});

	const { mutateAsync: updateExpenseCategory } = useUpdateExpenseCategory({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getHasBeenSuccessfullyUpdatedMessage(EXPENSE_CATEGORY_ENTITY_NAME)
				)
			);
			handleCloseExpenseForm();
			invalidateExpenseQueries(queryClient);
		},
		onError: createOnErrorHandler(toast),
	});

	const editingExpense = useMemo(() => {
		if (expenseId == null) {
			return undefined;
		}

		return rows.find((row) => String(row.id) === String(expenseId));
	}, [rows, expenseId]);

	const expenseEditingInitialValues = useMemo(() => {
		if (editingExpense) {
			return { expense: editingExpense.expense };
		}

		return { expense: '' };
	}, [editingExpense]);

	const handleEdit = useCallback(
		(row: ExpensesGridRow) => {
			handleOpenExpenseForm(Number(row.id));
		},
		[handleOpenExpenseForm]
	);

	const handleSubmitExpenseForm = useCallback(
		async (values: ExpenseFormValues) => {
			const payload = { categoryName: values.expense };

			if (expenseId != null) {
				await updateExpenseCategory({ id: expenseId, ...payload });

				return;
			}

			await createExpenseCategory(payload);
		},
		[createExpenseCategory, expenseId, updateExpenseCategory]
	);

	return {
		expenseId,
		isExpenseFormOpen,
		handleOpenExpenseForm,
		handleCloseExpenseForm,
		expenseEditingInitialValues,
		handleSubmitExpenseForm,
		handleEdit,
	};
};

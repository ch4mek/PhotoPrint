import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import { updateExpensesCache } from '../utils/updateExpensesCache';

import { useSaveExpense } from '@/queries/expenses/useSaveExpense';

export const useExpensesSaveExpense = () => {
	const queryClient = useQueryClient();

	const {
		mutateAsync: saveExpense,
		isPending,
		variables,
	} = useSaveExpense({
		onSuccess: (savedExpense, saveVariables) => {
			updateExpensesCache(queryClient, saveVariables, savedExpense);
		},
	});

	const isSavingExpense = useCallback(
		(expenseCategoryId: number, monthIndex: number) =>
			isPending &&
			variables?.expenseCategoryId === expenseCategoryId &&
			variables?.monthOfYear === monthIndex + 1,
		[isPending, variables]
	);

	return {
		saveExpense,
		isSavingExpense,
	};
};

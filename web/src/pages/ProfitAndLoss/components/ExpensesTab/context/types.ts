import { UseMutateAsyncFunction } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { Dispatch, SetStateAction } from 'react';

import { ServerResponseError } from '@/queries/types';
import { MonthExpense, SaveExpenseVariables } from '@/types/expenses';

export interface ExpensesTabContextValues {
	year: string;
	setYear: Dispatch<SetStateAction<string>>;
	resetYear: () => void;
	isResetDisabled: boolean;
	saveExpense: UseMutateAsyncFunction<
		MonthExpense,
		AxiosError<ServerResponseError>,
		SaveExpenseVariables
	>;
	isSavingExpense: (expenseCategoryId: number, monthIndex: number) => boolean;
}

import { createContext, PropsWithChildren, useContext, useMemo } from 'react';

import { useExpensesSaveExpense } from '../hooks/useExpensesSaveExpense';
import { useYearValueFilter } from '../hooks/useYearValueFilter';

import { ExpensesTabContextValues } from './types';

const ExpensesTabContext = createContext({} as ExpensesTabContextValues);

export function ExpensesTabProvider({ children }: PropsWithChildren) {
	const { year, setYear, resetYear, isResetDisabled } = useYearValueFilter();
	const { saveExpense, isSavingExpense } = useExpensesSaveExpense();

	const value = useMemo(
		() => ({
			year,
			setYear,
			resetYear,
			isResetDisabled,
			saveExpense,
			isSavingExpense,
		}),
		[year, setYear, resetYear, isResetDisabled, saveExpense, isSavingExpense]
	);

	return (
		<ExpensesTabContext.Provider value={value}>
			{children}
		</ExpensesTabContext.Provider>
	);
}

export const useExpensesTabContext = () => useContext(ExpensesTabContext);

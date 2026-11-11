import { EDIT_COLUMN_WIDTH } from '@/components/Grid/constants';
import { months } from '@/constants/date';
import { GridColumnWidthsConfiguration } from '@/types/grid';

import { ExpensesFilters } from './types';

export const MINIMAL_MONTH_COLUMN_WIDTH = 94.7;

export const expensesGridColumnsWidths = {
	expense: { width: 140, fixed: true },
} satisfies GridColumnWidthsConfiguration;

export const FIXED_COLUMNS_WIDTH =
	expensesGridColumnsWidths.expense.width + EDIT_COLUMN_WIDTH;

export const EXPENSES_GRID_EDIT_COLUMN_INDEX =
	months.length + Object.keys(expensesGridColumnsWidths).length + 2;

export const expensesGridColumns = {
	expense: {
		key: 'expense',
		name: 'Expense',
		sortable: true,
	},
};

export const EXPENSES_ORDERING_FIELD = 'OrderWeight';

export const initialExpensesFilters: ExpensesFilters = {
	expense: '',
};

export const expensesMenuOptionsValues = {
	edit: 'Edit',
	delete: 'Delete',
} as const;

export const EXPENSE_CELL_CLASS = 'expense-cell';

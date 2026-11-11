import { RenderCellProps } from 'react-data-grid';

import {
	ExpensesGridRow,
	ExpensesSummaryGridRow,
} from '@/pages/ProfitAndLoss/components/ExpensesTab/types';

export type MonthCellRendererProps = RenderCellProps<
	ExpensesGridRow,
	ExpensesSummaryGridRow
> & { width: number };

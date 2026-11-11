import { Typography } from '@mms/mms-ui-library/components/Typography';
import { RenderSummaryCellProps } from 'react-data-grid';

import { months } from '@/constants/date';
import {
	ExpensesGridRow,
	ExpensesSummaryGridRow,
} from '@/pages/ProfitAndLoss/components/ExpensesTab/types';

import { TotalCellWrapper } from './styles';

export const TotalCellRenderer = (
	props: RenderSummaryCellProps<ExpensesSummaryGridRow, ExpensesGridRow>
) => {
	const {
		row,
		column: { key },
	} = props;
	const monthIndex = months.findIndex((month) => month === key);
	const value = row.values?.[monthIndex];

	return (
		<TotalCellWrapper>
			<Typography variant="xs-400">{value}</Typography>
		</TotalCellWrapper>
	);
};

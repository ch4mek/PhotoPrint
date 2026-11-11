import { EditableCellRenderer } from '../EditableCellRenderer';

import { months } from '@/constants/date';
import { formatPrice } from '@/pages/ProfitAndLoss/utils/formatPrice';

import { MonthCellWrapper } from './styles';
import { MonthCellRendererProps } from './types';

export const MonthCellRenderer = ({
	row,
	column: { key },
	width,
}: MonthCellRendererProps) => {
	const index = months.findIndex((month) => month === key);
	const { id: expenseCategoryId } = row;

	const monthExpense = row.expensesByMonth?.find(
		(exp) => exp.monthOfYear - 1 === index
	);
	const initialValue = monthExpense?.expenseAmount;

	return (
		<MonthCellWrapper data-recordid={expenseCategoryId} cellWidth={width}>
			<EditableCellRenderer
				expenseCategoryId={expenseCategoryId}
				expenseId={monthExpense?.id}
				value={formatPrice(initialValue)}
				initialValue={initialValue}
				month={index}
			/>
		</MonthCellWrapper>
	);
};

import { RenderSummaryCellProps } from 'react-data-grid';

import { BYN_CURRENCY_SYMBOL } from '@/components/CurrencySymbol/constants';
import { BYNCurrencySymbol } from '@/components/CurrencySymbol/styles';
import { managementGridColumns } from '@/pages/ProfitAndLoss/components/ManagementTab/constants';
import { ManagementGridRow } from '@/pages/ProfitAndLoss/components/ManagementTab/types';
import { checkIsManagementSummaryRow } from '@/pages/ProfitAndLoss/components/ManagementTab/utils';
import { formatPrice } from '@/pages/ProfitAndLoss/utils/formatPrice';

import {
	SUMMARY_EXPENSES_LABEL,
	SUMMARY_EXPENSES_SUM_CODE,
	SUMMARY_PROFIT_LABEL,
	SUMMARY_VALUE_SUFFIX,
} from './constants';
import {
	CellContent,
	SummaryExpensesProfitCell,
	SummarySymbol,
} from './styles';

export const SummaryExpensesProfitCellRenderer = ({
	row,
	column,
}: RenderSummaryCellProps<unknown, ManagementGridRow>) => {
	if (
		column.key !== managementGridColumns.expand.key ||
		!checkIsManagementSummaryRow(row)
	) {
		return null;
	}

	return (
		<SummaryExpensesProfitCell>
			<CellContent variant="xs-400" Component="p">
				<span>
					<SummarySymbol>{SUMMARY_EXPENSES_SUM_CODE}</SummarySymbol>{' '}
					{SUMMARY_EXPENSES_LABEL}{' '}
					<BYNCurrencySymbol>{BYN_CURRENCY_SYMBOL}</BYNCurrencySymbol>
					{SUMMARY_VALUE_SUFFIX}
				</span>
				<span>{formatPrice(row.expenses)}</span>
			</CellContent>
			<CellContent variant="xs-400" Component="p">
				<span>
					{SUMMARY_PROFIT_LABEL}{' '}
					<BYNCurrencySymbol>{BYN_CURRENCY_SYMBOL}</BYNCurrencySymbol>
					{SUMMARY_VALUE_SUFFIX}
				</span>
				<span>{formatPrice(row.profit)}</span>
			</CellContent>
		</SummaryExpensesProfitCell>
	);
};

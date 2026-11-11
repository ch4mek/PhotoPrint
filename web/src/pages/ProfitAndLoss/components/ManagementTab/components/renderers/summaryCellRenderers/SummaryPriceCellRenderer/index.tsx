import { Typography } from '@mms/mms-ui-library/components/Typography';
import { RenderSummaryCellProps } from 'react-data-grid';

import { ManagementGridRow } from '@/pages/ProfitAndLoss/components/ManagementTab/types';
import { checkIsManagementSummaryRow } from '@/pages/ProfitAndLoss/components/ManagementTab/utils';
import { formatPrice } from '@/pages/ProfitAndLoss/utils/formatPrice';

import { SummaryPriceCellContent } from './styles';
import { SummaryPriceField } from './types';

export function createSummaryPriceCellRenderer(field: SummaryPriceField) {
	return function SummaryPriceCellRenderer({
		row,
		column,
	}: RenderSummaryCellProps<unknown, ManagementGridRow>) {
		if (column.key !== field || !checkIsManagementSummaryRow(row)) {
			return null;
		}

		const value = row[field];

		return (
			<SummaryPriceCellContent>
				<Typography variant="xs-400">{formatPrice(value)}</Typography>
			</SummaryPriceCellContent>
		);
	};
}

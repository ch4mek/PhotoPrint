import { RenderCellProps } from 'react-data-grid';

import { ManagementGridRow } from '@/pages/ProfitAndLoss/components/ManagementTab/types';
import { formatPrice } from '@/pages/ProfitAndLoss/utils/formatPrice';

import { PriceCellContent } from './styles';
import { PriceField } from './types';

export function createPriceCellRenderer(field: PriceField) {
	return function PriceCellRenderer({
		row,
	}: RenderCellProps<ManagementGridRow>) {
		const value = row[field];

		if (value === null) {
			return <PriceCellContent disabled={row.disabled}>-</PriceCellContent>;
		}

		return (
			<PriceCellContent disabled={row.disabled}>
				{formatPrice(value)}
			</PriceCellContent>
		);
	};
}

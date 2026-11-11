import { RenderCellProps } from 'react-data-grid';

import { editCellRenderer } from '@/components/Grid';
import { RatesGridRow } from '@/pages/ProfitAndLoss/components/RatesTab/types';

export function RateHistoryCellRenderer({
	row,
	...renderCellProps
}: RenderCellProps<RatesGridRow>) {
	const { isParent, hasChildren, handleEditClick, isActive } = row;
	const shouldShowEditButton =
		Boolean(handleEditClick) && isActive && (!isParent || !hasChildren);

	if (!shouldShowEditButton) {
		return null;
	}

	return editCellRenderer({ row, ...renderCellProps });
}

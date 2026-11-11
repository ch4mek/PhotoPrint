import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { HistoryCellWrapper } from '../styles';

import { GridCellDefaultProps } from '@/components/Grid';
import { HistoryTabs } from '@/forms/EquipmentForm/constants';

export const historyCellRenderer = <T extends GridCellDefaultProps>(
	{ row, column }: RenderCellProps<T, unknown>,
	activeTab: HistoryTabs,
	fieldToDisplay?: keyof T
) => {
	const fieldValue = row[fieldToDisplay || (column.key as keyof T)] as string;

	if (row.showSkeleton) {
		return <Skeleton animation="wave" width="100%" />;
	}

	return (
		<HistoryCellWrapper
			variant="s-400"
			isDeletedHistory={activeTab === HistoryTabs.DELETED}
		>
			{fieldValue}
		</HistoryCellWrapper>
	);
};

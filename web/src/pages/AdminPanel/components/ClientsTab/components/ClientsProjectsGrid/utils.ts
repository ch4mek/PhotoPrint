import { Column } from 'react-data-grid';

import { ClientsGridRow } from '../../types';

import { getNestedColumnRenderer } from '@/components/Grid/factories/getNestedColumnRenderer';
import { createTrackedCellRenderer } from '@/components/Grid/renderers/cell/trackedCellRenderer/trackedCellRenderer';

export const getClientProjectGridColumns = (
	displayTrackedColumn: boolean
): Column<ClientsGridRow>[] => {
	const columns: Column<ClientsGridRow>[] = [
		getNestedColumnRenderer({
			key: 'name',
			name: 'Client',
		}),
	];

	if (displayTrackedColumn) {
		columns.push(
			getNestedColumnRenderer({
				key: 'factTrackedTimeInMinutes',
				name: 'Tracked fact',
				renderCell: createTrackedCellRenderer(
					(row) => row.factTrackedTimeInMinutes,
					true
				),
			})
		);

		columns.push(
			getNestedColumnRenderer({
				key: 'billableTrackedTimeInMinutes',
				name: 'Tracked billable',
				renderCell: createTrackedCellRenderer(
					(row) => row.billableTrackedTimeInMinutes,
					true
				),
			})
		);
	}

	return columns;
};

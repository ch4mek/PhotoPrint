import { RatesGridRow } from '../types';

import { RATES_CHILD_ROW_HEIGHT } from '@/pages/ProfitAndLoss/constants';

const patchParentRow = (row: RatesGridRow, expandedIds: Set<string>) => {
	if (!row.isParent || !row.hasChildren) {
		return null;
	}

	const isExpanded = expandedIds.has(row.id);

	if (row.expanded === isExpanded) {
		return row;
	}

	return { ...row, expanded: isExpanded };
};

const patchChildRow = (row: RatesGridRow, expandedIds: Set<string>) => {
	if (row.isParent || !row.parentId) {
		return null;
	}

	const isExpanded = expandedIds.has(row.parentId);
	const itemRowHeight = isExpanded ? RATES_CHILD_ROW_HEIGHT : 0;

	if (row.itemRowHeight === itemRowHeight) {
		return row;
	}

	return { ...row, itemRowHeight };
};

export const patchRowsExpandedState = (
	rows: RatesGridRow[],
	expandedIds: Set<string>
) => {
	let hasChanges = false;

	const patchedRows = rows.map((row) => {
		const patchedRow =
			patchParentRow(row, expandedIds) ?? patchChildRow(row, expandedIds) ?? row;

		if (patchedRow !== row) {
			hasChanges = true;
		}

		return patchedRow;
	});

	return hasChanges ? patchedRows : rows;
};

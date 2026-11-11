import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { Dispatch, SetStateAction } from 'react';
import { SortColumn } from 'react-data-grid';

import {
	AbsenceTransactionHistoryGridColumns,
	AbsenceTransactionHistoryGridRow,
} from '@/types/absences';
import { GridColumnWidth } from '@/types/grid';

export interface AbsenceTransactionHistoryProps {
	rows: AbsenceTransactionHistoryGridRow[];
	setRows: Dispatch<SetStateAction<AbsenceTransactionHistoryGridRow[]>>;
	sortColumns: SortColumn[];
	handleSortColumnChange:
		| ((sortColumns: SortColumn[]) => void)
		| undefined
		| null;
	gridPagination?: GridPaginationProps;
	isPending: boolean;
}

export type AbsenceTransactionHistoryColumnWidths = Record<
	AbsenceTransactionHistoryGridColumns,
	GridColumnWidth
>;

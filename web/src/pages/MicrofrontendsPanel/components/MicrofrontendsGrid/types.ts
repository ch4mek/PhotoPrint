import { Column, SortColumn } from 'react-data-grid';

import { UserClaims } from '@/types/claims';

export interface MicrofrontendsGridProps {
	columns: Array<Column<MicrofrontendsGridRow>>;
	rows: MicrofrontendsGridRow[];
	isFetching: boolean;
	sortColumns: SortColumn[];
	setRows: (rows: MicrofrontendsGridRow[]) => void;
	handleSortColumnChange: (sortCols: SortColumn[]) => void;
	getRowHeight: (row: MicrofrontendsGridRow) => number;
	handleOpen: (itemId?: string | number) => void;
}

export interface MicrofrontendsGridRow {
	name: string;
	scope: string;
	modules: string[] | null;
	routes: string[] | null;
	version: string;
	remoteEntries: string | null;
	itemRowHeight: number;
	claims?: UserClaims[] | null;
	handleEdit: (itemId?: string | number) => () => void;
	handleDelete: (itemId?: string | number) => () => void;
}

import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { SortColumn } from 'react-data-grid';

import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import {
	AbsenceRequestFilters,
	AbsenceRequestGridColumns,
	AbsenceRequestGridRow,
	AbsenceRequestsFilterHandlers,
	UserAbsenceItem,
} from '@/types/absences';
import { GridColumnWidth } from '@/types/grid';

export interface AbsenceRequestsProps {
	absencesOptions: UserAbsenceItem[];
	filters: AbsenceRequestFilters;
	setFilters: AbsenceRequestsFilterHandlers;
	rows: AbsenceRequestGridRow[];
	sortColumns: SortColumn[];
	handleSortColumnChange:
		| ((sortColumns: SortColumn[]) => void)
		| undefined
		| null;
	gridPagination?: GridPaginationProps;
	isPending: boolean;
}

export type AbsenceRequestColumnWidths = Record<
	AbsenceRequestGridColumns,
	GridColumnWidth
>;

export interface AbsenceRequestFiltersOptions {
	absenceTypes: Array<SelectionModalOption>;
	statuses: Array<SelectionModalOption>;
}

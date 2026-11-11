import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { SortColumn } from 'react-data-grid';

import { FromToProps, GridCellDefaultProps } from '@/components/Grid';

export interface PayslipsTableProps {
	pagination?: GridPaginationProps;
	rows: Array<PayslipRow>;
	areFiltersApplied?: boolean;
	onFiltersReset?: () => void;
	filters?: PayslipsTableFilters;
	filtersSetter?: PayslipsFiltersSetter;
	sortColumns?: Array<SortColumn>;
	onSortColumnChange?: (newSortColumns: Array<SortColumn>) => void;
}

export interface PayslipRow extends GridCellDefaultProps {
	fileId: string;
	reportDate: string;
	fileName: string;
}

export interface PayslipsTableFilters {
	reportDate: FromToProps<Date>;
}

export interface PayslipsFiltersSetter {
	reportDate: (newDate: FromToProps<Date>) => void;
	set: (newFilters: PayslipsTableFilters) => void;
}

export enum PayslipsTableFiltersKeys {
	reportDate = 'reportDate',
	set = 'set',
}

export interface PayslipsAction {
	type: PayslipsTableFiltersKeys;
	payload?: FromToProps<Date> | PayslipsTableFilters;
}

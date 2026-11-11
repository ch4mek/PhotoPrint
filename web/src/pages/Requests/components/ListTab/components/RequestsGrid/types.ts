import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { SortColumn } from 'react-data-grid';

import { OptionsProps, RequestFilterType, RequestGridRow } from '../../types';
import { generateSetFilters } from '../../utils';

export interface RequestGridProps {
	rows: RequestGridRow[];
	sortColumns: SortColumn[];
	handleSortColumnChange:
		| ((sortColumns: SortColumn[]) => void)
		| undefined
		| null;
	filters: RequestFilterType;
	setFilters: ReturnType<typeof generateSetFilters>;
	options: OptionsProps;
	paginationProps?: GridPaginationProps;
}

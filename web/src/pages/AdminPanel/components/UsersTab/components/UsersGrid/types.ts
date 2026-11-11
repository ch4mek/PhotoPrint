import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { SortColumn } from 'react-data-grid';

import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import {
	PositionOption,
	UsersFilterType,
	UsersOptions,
} from '@/pages/AdminPanel/components/UsersTab/types';
import { generateSetFilters } from '@/pages/AdminPanel/components/UsersTab/utils';
import { GridColumnWidthsConfiguration } from '@/types/grid';
import { UsersGridRow } from '@/types/userInfo';

interface Options {
	defaultOptions: UsersOptions;
	positionOptions: PositionOption[];
	departmentOptions: SelectionModalOption[];
}

export interface UsersGridProps {
	rows: UsersGridRow[];
	options: Options;
	columnsWidths: GridColumnWidthsConfiguration;
	sortColumns: SortColumn[];
	filters: UsersFilterType;
	setFilters: ReturnType<typeof generateSetFilters>;
	initialFilters: UsersFilterType;
	handleSortColumnChange:
		| ((sortColumns: SortColumn[]) => void)
		| undefined
		| null;
	handleToggleUserActive: (row: UsersGridRow) => void;
	handleCrossLinkClick: (id?: string | number) => VoidFunction | undefined;
	enableCellClick: (row: UsersGridRow) => boolean;
	paginationProps?: GridPaginationProps;
}

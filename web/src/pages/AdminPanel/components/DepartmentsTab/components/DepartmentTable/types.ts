import { DepartmentWithGroupManagersFlag, RowsMenuOptions } from '../../types';

import { GridCellDefaultProps } from '@/components/Grid';

export interface DepartmentsTableProps {
	data?: DepartmentWithGroupManagersFlag[];
	menuOptions: RowsMenuOptions;
}

interface BaseCollapsibleCellWithMenuProps {
	isExpanded?: boolean;
}

export type CollapsibleCellWithMenuProps = Record<string, any> &
	GridCellDefaultProps &
	BaseCollapsibleCellWithMenuProps;

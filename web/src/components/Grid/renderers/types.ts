import { ReactNode } from 'react';

import { Roles } from '@/constants/roles';

export type HandleGridItemClick = (
	itemId?: string | number
) => VoidFunction | undefined;

export interface GridCellDefaultProps {
	id?: string | number;
	showSkeleton?: boolean;
	role?: Roles;
	handleEditClick?: HandleGridItemClick;
	disabled?: boolean;
	clearFilter?: () => void;
	showCell?: boolean;
	copyEnabled?: boolean;
	displayFilterIcon?: boolean;
	logicalTextOptions?: [string, string];
	tooltipContent?: ReactNode;
}

export interface DynamicHeightCellDefaultProps extends GridCellDefaultProps {
	height?: number;
	verticalEllipsisLineCount?: number;
}

export interface GridCellWithTrackedTimeProps {
	showSkeleton?: boolean;
	isActive?: boolean;
}

export interface CellTextProps {
	disabled?: boolean;
	padding?: string | number;
	verticalEllipsisLineCount?: number;
	maxWidth?: string;
	height?: string;
	width?: string;
}

export interface LogicalGridCellDefaultProps extends GridCellDefaultProps {
	logicalTextOptions: [string, string];
}

export interface ExpandableGridCellProps extends GridCellDefaultProps {
	expanded?: boolean;
	hasChildren?: boolean;
}

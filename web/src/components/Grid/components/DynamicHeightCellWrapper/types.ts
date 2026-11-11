import { PropsWithChildren } from 'react';

import { GridCellDefaultProps } from '@/components/Grid';

export interface DynamicHeightWrapperProps<T> extends PropsWithChildren {
	row: T;
	idx: number;
	onRowChange: (row: T) => void;
	showSkeleton?: boolean;
	maxTextRowCount?: number;
	tooltipText?: string;
	tooltip?: boolean;
}

export interface GridCellPropsWithHeight extends GridCellDefaultProps {
	id?: string | number;
	height?: number;
}

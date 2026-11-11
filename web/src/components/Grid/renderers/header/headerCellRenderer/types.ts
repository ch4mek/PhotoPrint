import { ReactNode } from 'react';
import { RenderHeaderCellProps } from 'react-data-grid';

import { GridCellDefaultProps } from '../../types';

export type HeaderCellRendererProps<
	T extends GridCellDefaultProps,
	TSummaryRow = unknown
> = RenderHeaderCellProps<T, TSummaryRow> & {
	nameContent?: ReactNode;
	tooltipContent?: ReactNode;
	isContentAlignedToEnd?: boolean;
};

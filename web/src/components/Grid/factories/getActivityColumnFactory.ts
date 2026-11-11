import { Column } from 'react-data-grid';

import { headerCellRenderer } from '../renderers';
import { logicalTextCellRenderer } from '../renderers/cell/activityCellRenderer';
import { GridCellDefaultProps } from '../renderers/types';

type LogicalColumn<T> = Column<T> & { logicalTextOptions: [string, string] };

export const getLogicalTextColumnFactory = <T extends GridCellDefaultProps>({
	logicalTextOptions,
	renderHeaderCell = headerCellRenderer,
	renderCell = logicalTextCellRenderer(logicalTextOptions),
	...otherProps
}: LogicalColumn<T>) => ({
	renderHeaderCell,
	renderCell,
	...otherProps,
});

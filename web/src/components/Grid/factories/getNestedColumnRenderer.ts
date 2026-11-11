import { Column } from 'react-data-grid';

import {
	GridCellDefaultProps,
	headerCellRenderer,
	nestedCellRenderer,
} from '../renderers';

export const getNestedColumnRenderer = <T extends GridCellDefaultProps>({
	renderHeaderCell = headerCellRenderer,
	renderCell = nestedCellRenderer,
	...otherProps
}: Column<T>) => ({
	renderHeaderCell,
	renderCell,
	...otherProps,
});

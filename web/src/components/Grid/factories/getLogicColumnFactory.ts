import { Column } from 'react-data-grid';

import {
	headerCellRenderer,
	logicCellRenderer,
	GridCellDefaultProps,
} from '../renderers';

export const getLogicColumnFactory = <T extends GridCellDefaultProps>({
	renderHeaderCell = headerCellRenderer,
	renderCell = logicCellRenderer,
	...otherProps
}: Column<T>) => ({
	renderHeaderCell,
	renderCell,
	...otherProps,
});

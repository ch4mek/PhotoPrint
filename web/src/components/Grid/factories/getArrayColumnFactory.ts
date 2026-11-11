import { Column } from 'react-data-grid';

import {
	GridCellDefaultProps,
	arrayCellRenderer,
	headerCellRenderer,
} from '../renderers';

export const getArrayColumnFactory = <T extends GridCellDefaultProps>({
	renderHeaderCell = headerCellRenderer,
	renderCell = arrayCellRenderer,
	cellClass,
	...otherProps
}: Column<T>) => {
	const innerCellClass = `rdg-cell-array ${cellClass}`;

	return {
		cellClass: innerCellClass,
		renderHeaderCell,
		renderCell,
		...otherProps,
	};
};

import { Column } from 'react-data-grid';

import {
	CellWithColorCircleProps,
	cellWithColorCircleRenderer,
	headerCellRenderer,
} from '../renderers';

export const getColumnWithColorCircleFactory = <
	T extends CellWithColorCircleProps
>({
	renderHeaderCell = headerCellRenderer,
	renderCell = cellWithColorCircleRenderer,
	...otherProps
}: Column<T>) => ({
	renderHeaderCell,
	renderCell,
	...otherProps,
});

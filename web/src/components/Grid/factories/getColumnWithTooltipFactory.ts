import { Column } from 'react-data-grid';

import {
	GridCellDefaultProps,
	cellWithTooltipRenderer,
	headerCellRenderer,
} from '../renderers';

export const getColumnWithTooltipFactory = <T extends GridCellDefaultProps>({
	renderHeaderCell = headerCellRenderer,
	renderCell = cellWithTooltipRenderer,
	...otherProps
}: Column<T>) => ({
	renderHeaderCell,
	renderCell,
	...otherProps,
});

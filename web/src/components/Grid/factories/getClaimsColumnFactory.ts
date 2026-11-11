import { Column } from 'react-data-grid';

import { headerCellRenderer } from '../renderers';
import { claimCellRenderer } from '../renderers/cell/claimCellRenderer';
import { GridCellDefaultProps } from '../renderers/types';

export const getClaimsColumnFactory = <T extends GridCellDefaultProps>({
	renderHeaderCell = headerCellRenderer,
	renderCell = claimCellRenderer,
	...otherProps
}: Column<T>) => ({
	renderHeaderCell,
	renderCell,
	...otherProps,
});

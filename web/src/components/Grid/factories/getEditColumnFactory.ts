import { Column } from 'react-data-grid';

import { EDIT_COLUMN_CELL_CLASS, EDIT_COLUMN_HEADER_CLASS } from '../constants';
import { GridCellDefaultProps, editCellRenderer } from '../renderers';
import { headerCellWithDisabledFilterRenderer } from '../renderers/header/headerCellWithDisabledFilterRenderer';

interface ExtendedColumn<T> extends Column<T> {
	disabled: boolean;
	displayFilterButton?: boolean;
	clearFilter?: () => void;
}

export const getEditColumnFactory = <T extends GridCellDefaultProps>({
	disabled,
	displayFilterButton,
	headerCellClass = EDIT_COLUMN_HEADER_CLASS,
	cellClass = EDIT_COLUMN_CELL_CLASS,
	clearFilter = () => null,
	renderHeaderCell = headerCellWithDisabledFilterRenderer({
		disabled,
		onClick: clearFilter,
		displayFilterButton,
	}),
	renderCell = editCellRenderer,
	...otherProps
}: ExtendedColumn<T>) => ({
	renderHeaderCell,
	renderCell,
	headerCellClass,
	cellClass,
	...otherProps,
});

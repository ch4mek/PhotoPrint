import {
	ELEMENTS_TAB_GRID_COLUMNS,
	TWO_COLUMNS_GRID_CORRECTION,
} from '../constants';

export const getColumnIndexForStyle = () =>
	Object.keys(ELEMENTS_TAB_GRID_COLUMNS).length + TWO_COLUMNS_GRID_CORRECTION;

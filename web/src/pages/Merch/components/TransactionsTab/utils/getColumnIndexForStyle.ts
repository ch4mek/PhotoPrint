import { TWO_COLUMNS_GRID_CORRECTION } from '../../ElementsTab/constants';
import { TRANSACTIONS_TAB_GRID_COLUMNS } from '../constants';

export const getColumnIndexForStyle = () =>
	Object.keys(TRANSACTIONS_TAB_GRID_COLUMNS).length +
	TWO_COLUMNS_GRID_CORRECTION;

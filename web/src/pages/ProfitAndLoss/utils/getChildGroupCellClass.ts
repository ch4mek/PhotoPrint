import { RATES_CHILD_GROUP_CELL_CLASS } from '../constants';
import { ProfitAndLossBaseGridRow } from '../types';

export const getChildGroupCellClass = (
	row: ProfitAndLossBaseGridRow,
	baseClass = ''
) => {
	if (row.isParent) {
		return baseClass;
	}

	return baseClass
		? `${baseClass} ${RATES_CHILD_GROUP_CELL_CLASS}`
		: RATES_CHILD_GROUP_CELL_CLASS;
};

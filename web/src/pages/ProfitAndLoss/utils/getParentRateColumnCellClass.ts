import { RATES_RATE_COLUMN_HEADER_CLASS } from '../constants';
import { ProfitAndLossBaseGridRow } from '../types';

export const getParentRateColumnCellClass = (
	row: ProfitAndLossBaseGridRow,
	baseClass = ''
) => {
	if (!row.isParent) {
		return baseClass;
	}

	return baseClass
		? `${baseClass} ${RATES_RATE_COLUMN_HEADER_CLASS}`
		: RATES_RATE_COLUMN_HEADER_CLASS;
};

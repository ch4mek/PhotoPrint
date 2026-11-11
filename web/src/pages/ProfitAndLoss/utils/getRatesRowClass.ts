import { ratesRowClasses } from '../constants';
import { ProfitAndLossGridRowClass } from '../types';

export const getRatesRowClass = (row: ProfitAndLossGridRowClass) => {
	const classes: string[] = [];

	if (!row.isActive) {
		classes.push(ratesRowClasses.inactiveUser);
	}

	if (row.isParent) {
		classes.push(ratesRowClasses.parentRow);

		if (row.expanded) {
			classes.push(ratesRowClasses.parentRowExpanded);
		}
	} else {
		classes.push(ratesRowClasses.childRow);

		if (row.isLastChild) {
			classes.push(ratesRowClasses.childRowLast);
		}
	}

	if (row.parentIndex % 2 === 0) {
		classes.push(ratesRowClasses.rowEven);
	} else {
		classes.push(ratesRowClasses.rowOdd);
	}

	return classes.join(' ');
};

import { FromToProps } from '@/components/Grid';

import { areFromToCompatibleObjectsEqual } from './areValuesEqual';
import { isDateRangeValid } from './isDateRangeValid';

export const isDateRangeApplyDisabled = (
	previousSelection: FromToProps<Date>,
	currentSelection: FromToProps<Date>,
	allowEqual: boolean = true
): boolean => {
	const { from, to } = currentSelection;

	if (!from && !to) {
		return true;
	}

	return from && to
		? !isDateRangeValid(from, to, allowEqual) ||
				areFromToCompatibleObjectsEqual(currentSelection, previousSelection)
		: areFromToCompatibleObjectsEqual(currentSelection, previousSelection);
};

import { FocusEvent } from 'react';

export const checkIsFocusOnCurrentDoubleCell = (
	e: FocusEvent<HTMLInputElement>
): boolean => {
	const parentCell = e.currentTarget.closest("[data-cell='double-cell']");

	if (!parentCell) {
		return false;
	}

	const allInputsInCell = parentCell.querySelectorAll('input') || [];
	const isFactInputInCell = Array.from(allInputsInCell).some(
		(input) => input === e.target
	);
	const isAnyInputFocused = Array.from(allInputsInCell).some(
		(input) => input === e.relatedTarget
	);

	return isFactInputInCell && isAnyInputFocused;
};

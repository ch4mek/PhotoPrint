export const getNumericIdForAddedItem = (rows: Array<{ id: number }>): number =>
	Math.max(...rows.map(({ id: planId }) => planId), 1) + 1;

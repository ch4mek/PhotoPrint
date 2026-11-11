export const getSkeletonRows = <T>(count: number, mockRow: T) =>
	new Array(count).fill(mockRow);

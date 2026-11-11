export const getMockRows = <T>(count: number, mockRow: T) =>
	new Array(count).fill(mockRow);

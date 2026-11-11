import { Theme } from '@emotion/react';

import { ROWS_PER_PAGE_OPTIONS } from '@/constants/pagination';
import { createGridPagination } from '@/helpers/createGridPagination';

describe('Helper function createGridPagination', () => {
	const onCurrentPageChange = jest.fn();
	const onPerPageChange = jest.fn();

	it('should return undefined if rowsLength is 0', () => {
		const result = createGridPagination(
			0,
			1,
			onCurrentPageChange,
			25,
			onPerPageChange,
			100
		);
		expect(result).toBeUndefined();
	});

	it('should return GridPaginationProps with default perPageOptions', () => {
		const result = createGridPagination(
			10,
			2,
			onCurrentPageChange,
			50,
			onPerPageChange,
			100
		);

		expect(result).toBeDefined();
		expect(result?.currentPage).toBe(2);
		expect(result?.perPage).toBe(50);
		expect(result?.total).toBe(100);
		expect(result?.perPageOptions).toEqual(ROWS_PER_PAGE_OPTIONS);
		expect(result?.onCurrentPageChange).toBe(onCurrentPageChange);
		expect(result?.onPerPageChange).toBe(onPerPageChange);
		expect(typeof result?.overridePaginationContainerStyles).toBe('function');
	});

	it('should allow overriding total and perPageOptions', () => {
		const customOptions = [10, 20, 30];
		const result = createGridPagination(
			5,
			1,
			onCurrentPageChange,
			10,
			onPerPageChange,
			50,
			customOptions
		);

		expect(result?.total).toBe(50);
		expect(result?.perPageOptions).toEqual(customOptions);
	});

	it('overridePaginationContainerStyles should return styles object with name and styles', () => {
		const result = createGridPagination(
			1,
			1,
			onCurrentPageChange,
			25,
			onPerPageChange
		);

		const themeMock = { spaces: { s: 8 } };
		const styles = result?.overridePaginationContainerStyles!(
			themeMock as Theme
		);

		expect(styles).toHaveProperty('name');
		expect(styles).toHaveProperty('styles');
		expect(styles?.styles).toContain('padding: 8px');
	});
});

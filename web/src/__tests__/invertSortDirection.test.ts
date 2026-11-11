import { SortDirection } from '@/constants/sort';
import { invertSortDirection } from '@/helpers/invertSortDirection';

describe('Helper function invertSortDirection', () => {
	it('Should invert ASC to DESC', () => {
		const result = invertSortDirection(SortDirection.ASC);
		expect(result).toBe(SortDirection.DESC);
	});

	it('Should invert DESC to ASC', () => {
		const result = invertSortDirection(SortDirection.DESC);
		expect(result).toBe(SortDirection.ASC);
	});
});

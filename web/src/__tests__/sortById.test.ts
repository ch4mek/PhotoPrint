import { sortById } from '@/helpers/sortById';

describe('sortById', () => {
	it('should sort items by id in ascending order', () => {
		const items = [
			{ id: 3, name: 'c' },
			{ id: 1, name: 'a' },
			{ id: 2, name: 'b' },
		];
		expect(sortById(items)).toEqual([
			{ id: 1, name: 'a' },
			{ id: 2, name: 'b' },
			{ id: 3, name: 'c' },
		]);
	});

	it('should not mutate the original array', () => {
		const items = [
			{ id: 2, name: 'b' },
			{ id: 1, name: 'a' },
		];
		const result = sortById(items);
		expect(items).toEqual([
			{ id: 2, name: 'b' },
			{ id: 1, name: 'a' },
		]);
		expect(result).not.toBe(items);
	});

	it('should return empty array for empty input', () => {
		expect(sortById([])).toEqual([]);
	});

	it('should return single item as is', () => {
		const items = [{ id: 1, name: 'a' }];
		expect(sortById(items)).toEqual([{ id: 1, name: 'a' }]);
	});

	it('should handle already sorted array', () => {
		const items = [
			{ id: 1, name: 'a' },
			{ id: 2, name: 'b' },
		];
		expect(sortById(items)).toEqual(items);
	});
});

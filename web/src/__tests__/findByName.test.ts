import { findByName } from '@/helpers/findByName';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

const items: SimpleConfigurableConfigurationItem<number>[] = [
	{ name: 'Alice', id: 1 },
	{ name: 'Bob', id: 2 },
	{ name: 'Charlie', id: 3 },
];

describe('findByName tests', () => {
	it('should return the correct item when the name matches', () => {
		const result = findByName(items, 'Bob');
		expect(result).toEqual({ name: 'Bob', id: 2 });
	});

	it('should return undefined when no name matches', () => {
		const result = findByName(items, 'Boris');
		expect(result).toBeUndefined();
	});

	it('should return undefined for an empty array', () => {
		const result = findByName([], 'Alice');
		expect(result).toBeUndefined();
	});

	it('should work with different data types for the name property', () => {
		type ItemWithNumberName = { name: number; id: number };
		const itemsWithNumberName: ItemWithNumberName[] = [
			{ name: 101, id: 1 },
			{ name: 202, id: 2 },
		];
		const result = findByName(itemsWithNumberName, 101);
		expect(result).toEqual({ name: 101, id: 1 });
	});
});

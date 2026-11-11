import { findById } from '@/helpers/findById';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

type TestItem = { id: number; name: string };

const items: SimpleConfigurableConfigurationItem<number>[] = [
	{ name: 'Alice', id: 1 },
	{ name: 'Bob', id: 2 },
	{ name: 'Charlie', id: 3 },
];

describe('findById tests', () => {
	it('should return the correct item when the id matches', () => {
		const result = findById(items, 2);
		expect(result).toEqual({ id: 2, name: 'Bob' });
	});

	it('should return undefined when no id matches', () => {
		const result = findById(items, 42);
		expect(result).toBeUndefined();
	});

	it('should return undefined for an empty array', () => {
		const result = findById([], 1);
		expect(result).toBeUndefined();
	});

	it('should work with string IDs', () => {
		const itemsWithStringId: SimpleConfigurableConfigurationItem<string>[] = [
			{ id: '1', name: 'Alice' },
			{ id: '2', name: 'Bob' },
		];
		const result = findById(itemsWithStringId, '2');
		expect(result).toEqual({ id: '2', name: 'Bob' });
	});

	it('should correctly handle duplicate IDs by returning the first match', () => {
		const itemsWithNoUniqueIds: TestItem[] = [
			{ id: 1, name: 'Alice' },
			{ id: 1, name: 'Bob' },
		];
		const result = findById(itemsWithNoUniqueIds, 1);
		expect(result).toEqual({ id: 1, name: 'Alice' });
	});
});

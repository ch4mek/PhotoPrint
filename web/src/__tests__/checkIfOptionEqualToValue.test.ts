import {
	areObjectsFieldEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import {
	checkIfOptionsEqualById,
	checkIfPrimitiveOptionsEqual,
} from '@/helpers/checkIfOptionEqualToValue';

type WithId = {
	id: number;
	name?: string;
};

describe('Helper function checkIfOptionEqualToValue', () => {
	describe('when used with field comparator', () => {
		const comparator = (a: WithId, b: WithId, key: keyof WithId) =>
			areObjectsFieldEqual(a, b, key);
		const check = (key: keyof WithId) => (a: WithId, b: WithId) =>
			comparator(a, b, key);

		it('should return true if objects have equal values for the given key', () => {
			const obj1 = { id: 1, name: 'Alice' };
			const obj2 = { id: 1, name: 'Bob' };

			expect(check('id')(obj1, obj2)).toBe(true);
		});

		it('should return false if objects have different values for the given key', () => {
			const obj1 = { id: 1, name: 'Alice' };
			const obj2 = { id: 2, name: 'Alice' };

			expect(check('id')(obj1, obj2)).toBe(false);
		});
	});

	describe('when used with primitive comparator', () => {
		const comparator = (a: string, b: string) => arePrimitiveValuesEqual(a, b);
		const check = (a: string, b: string) => comparator(a, b);

		it('should return true if primitive values are equal', () => {
			expect(check('hello', 'hello')).toBe(true);
		});

		it('should return false if primitive values are not equal', () => {
			expect(check('hello', 'world')).toBe(false);
		});
	});

	describe('checkIfOptionsEqualById', () => {
		it('should return true when two objects have the same id', () => {
			const obj1 = { id: 5, name: 'Alpha' };
			const obj2 = { id: 5, name: 'Beta' };

			expect(checkIfOptionsEqualById(obj1, obj2)).toBe(true);
		});

		it('should return false when two objects have different ids', () => {
			const obj1 = { id: 1, name: 'Alpha' };
			const obj2 = { id: 2, name: 'Alpha' };

			expect(checkIfOptionsEqualById(obj1, obj2)).toBe(false);
		});
	});

	describe('checkIfPrimitiveOptionsEqual', () => {
		it('should return true when two primitive values are equal', () => {
			expect(checkIfPrimitiveOptionsEqual(42, 42)).toBe(true);
		});

		it('should return false when two primitive values are different', () => {
			expect(checkIfPrimitiveOptionsEqual(42, 99)).toBe(false);
		});
	});
});

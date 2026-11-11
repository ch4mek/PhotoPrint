import {
	convertConstantToSelectionModalOption,
	convertToSelectionModalOptions,
} from '@/helpers/convertToSelectionModalOptions';

describe('Helper function convertToSelectionModalOptions', () => {
	describe('Helper function convertToSelectionModalOptions', () => {
		const mockItems = [
			{ id: '1', name: 'Alice', age: 25 },
			{ id: '2', name: 'Bob', age: 30 },
			{ id: '3', name: 'Charlie', age: 35 },
		];

		it('Should convert items array to SelectionModalOption using specified field', () => {
			const result = convertToSelectionModalOptions(mockItems, 'name');
			expect(result).toEqual([
				{ id: '1', description: 'Alice' },
				{ id: '2', description: 'Bob' },
				{ id: '3', description: 'Charlie' },
			]);
		});

		it('Should correctly convert numeric field to string in description', () => {
			const result = convertToSelectionModalOptions(mockItems, 'age');
			expect(result).toEqual([
				{ id: '1', description: '25' },
				{ id: '2', description: '30' },
				{ id: '3', description: '35' },
			]);
		});

		it('Should handle empty array and return empty array', () => {
			const result = convertToSelectionModalOptions([], 'name');
			expect(result).toEqual([]);
		});
	});

	describe('Helper function convertConstantToSelectionModalOption', () => {
		const constant = 'SOME_CONSTANT';

		it('Should convert a constant string to SelectionModalOption', () => {
			const result = convertConstantToSelectionModalOption(constant);
			expect(result).toEqual({ id: constant, description: constant });
		});
	});
});

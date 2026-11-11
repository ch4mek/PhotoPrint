import { getItemFromStorage, setItemToStorage } from '@/helpers/storage';

describe('Storage utilities', () => {
	let mockStorage: any;
	const mockId = 1;

	beforeEach(() => {
		mockStorage = {
			getItem: jest.fn(),
			setItem: jest.fn(),
		};
	});

	describe('getItemFromStorage', () => {
		it('should parse and return data from storage', () => {
			mockStorage.getItem.mockReturnValue('{"test": "value"}');

			const result = getItemFromStorage('key', mockId, mockStorage);

			expect(mockStorage.getItem).toHaveBeenCalledWith(`key/${mockId}`);
			expect(result).toEqual({ test: 'value' });
		});

		it('should return null when item does not exist', () => {
			mockStorage.getItem.mockReturnValue(null);

			const result = getItemFromStorage('key', mockId, mockStorage);

			expect(result).toBeNull();
		});
	});

	describe('setItemToStorage', () => {
		it('should stringify and store data', () => {
			const data = { test: 'value' };

			setItemToStorage('key', data, mockStorage);

			expect(mockStorage.setItem).toHaveBeenCalledWith(
				'key',
				'{"test":"value"}'
			);
		});

		it('should handle primitive values', () => {
			setItemToStorage('key', 'string', mockStorage);

			expect(mockStorage.setItem).toHaveBeenCalledWith('key', '"string"');
		});
	});
});

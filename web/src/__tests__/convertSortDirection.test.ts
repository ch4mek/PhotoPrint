import { SortDirection } from '@/constants/sort';
import {
	convertToDataGridSortDirection,
	convertToNativeSortDirection,
} from '@/helpers/convertSortDirection';

describe('Helper function convertSortDirection', () => {
	describe('convertToDataGridSortDirection', () => {
		it('should convert ASC to ASC', () => {
			expect(convertToDataGridSortDirection(SortDirection.ASC)).toBe('ASC');
		});

		it('should convert DESC to DESC', () => {
			expect(convertToDataGridSortDirection(SortDirection.DESC)).toBe('DESC');
		});

		it('should return ASC for invalid direction', () => {
			expect(convertToDataGridSortDirection('invalid' as any)).toBe('ASC');
		});
	});

	describe('convertToNativeSortDirection', () => {
		it('should convert ASC to internal ASC', () => {
			expect(convertToNativeSortDirection('ASC')).toBe(SortDirection.ASC);
		});

		it('should convert DESC to internal DESC', () => {
			expect(convertToNativeSortDirection('DESC')).toBe(SortDirection.DESC);
		});

		it('should return internal ASC for invalid direction', () => {
			expect(convertToNativeSortDirection('invalid' as any)).toBe(
				SortDirection.ASC
			);
		});
	});
});

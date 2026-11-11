import {
	formatAllowedFileTypesToAccept,
	formatAllowedFileTypesToShow,
} from '@/helpers/formatAllowedFileTypes';

describe('Helper function formatAllowedFileTypes', () => {
	describe('formatAllowedFileTypesToShow function', () => {
		it('Should format multiple extensions as comma-separated string', () => {
			const extensions = ['jpg', 'png', 'gif'];
			const result = formatAllowedFileTypesToShow(extensions);
			expect(result).toBe('JPG, PNG, GIF');
		});

		it('Should replace last comma with "and" when includeAndWordSeparator is true', () => {
			const extensions = ['jpg', 'png', 'gif'];
			const result = formatAllowedFileTypesToShow(extensions, true);
			expect(result).toBe('JPG, PNG and GIF');
		});

		it('Should handle single extension correctly', () => {
			const extensions = ['pdf'];
			const result = formatAllowedFileTypesToShow(extensions, true);
			expect(result).toBe('PDF');
		});

		it('Should handle empty array and return empty string', () => {
			const extensions: string[] = [];
			const result = formatAllowedFileTypesToShow(extensions, true);
			expect(result).toBe('');
		});

		it('Should handle includeAndWordSeparator false explicitly', () => {
			const extensions = ['doc', 'xls'];
			const result = formatAllowedFileTypesToShow(extensions, false);
			expect(result).toBe('DOC, XLS');
		});
	});

	describe('formatAllowedFileTypesToAccept', () => {
		it('Should format extensions into dot-prefixed accept string', () => {
			const extensions = ['jpg', 'png', 'gif'];
			const result = formatAllowedFileTypesToAccept(extensions);
			expect(result).toBe('.jpg, .png, .gif');
		});

		it('Should handle single extension correctly', () => {
			const extensions = ['pdf'];
			const result = formatAllowedFileTypesToAccept(extensions);
			expect(result).toBe('.pdf');
		});

		it('Should handle empty array and return empty string', () => {
			const extensions: string[] = [];
			const result = formatAllowedFileTypesToAccept(extensions);
			expect(result).toBe('');
		});
	});
});

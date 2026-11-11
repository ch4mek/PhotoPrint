import { convertBytesToKB, convertKBToMB } from '@/helpers/convertFileSize';

const FILE_SIZE = 1024;

describe('Helper function convertFileSize', () => {
	describe('convertBytesToKB', () => {
		it('Should convert bytes to kilobytes correctly', () => {
			const result = convertBytesToKB(FILE_SIZE);
			expect(result).toBe(1);
		});

		it('Should convert larger bytes to kilobytes correctly', () => {
			const result = convertBytesToKB(FILE_SIZE * 10);
			expect(result).toBe(10);
		});

		it('Should return 0 for 0 bytes', () => {
			const result = convertBytesToKB(0);
			expect(result).toBe(0);
		});

		it('Should handle negative values', () => {
			const result = convertBytesToKB(-FILE_SIZE);
			expect(result).toBe(-1);
		});
	});

	describe('convertKBToMB', () => {
		it('Should convert kilobytes to megabytes correctly', () => {
			const result = convertKBToMB(FILE_SIZE);
			expect(result).toBe(1);
		});

		it('Should return 0 for 0 KB', () => {
			const result = convertKBToMB(0);
			expect(result).toBe(0);
		});

		it('Should handle negative values', () => {
			const result = convertKBToMB(-FILE_SIZE);
			expect(result).toBe(-1);
		});
	});
});

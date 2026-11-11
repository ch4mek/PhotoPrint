import { generateImageDataUrl } from '@/helpers/generateImageDataUrl';

describe('generateImageDataUrl', () => {
	it('should return a image url with jpeg mime type', () => {
		const base = 'someBase64String';
		const result = generateImageDataUrl(base);
		expect(result).toBe(`data:image/jpeg;base64,${base}`);
	});

	it('should handle empty string input', () => {
		const result = generateImageDataUrl('');
		expect(result).toBe('data:image/jpeg;base64,');
	});

	it('should not add extra characters to the image url', () => {
		const base = '1234567890abcdef';
		const result = generateImageDataUrl(base);
		expect(result.startsWith('data:image/jpeg;base64,')).toBe(true);
		expect(result.endsWith(base)).toBe(true);
	});
});

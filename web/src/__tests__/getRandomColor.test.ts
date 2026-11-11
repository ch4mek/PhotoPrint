import { getRandomColor, randomColorsArray } from '@/helpers/getRandomColor';

const hexColorRegex = /^#[0-9A-F]{6}$/i;

describe('Helper function getRandomColor', () => {
	it('Should generate a random color in hexadecimal format', () => {
		const result = getRandomColor();
		expect(hexColorRegex.test(result)).toBe(true);
	});

	it('Should generate a random color with custom radix and substring length', () => {
		const result = getRandomColor(16, 2, 8);
		expect(hexColorRegex.test(result)).toBe(true);
	});
});

describe('Helper function randomColorsArray', () => {
	it('Should generate an array of random colors with the specified amount', () => {
		const randomColorsAmount = 5;
		const result = randomColorsArray(randomColorsAmount);

		expect(result).toHaveLength(randomColorsAmount);

		result.forEach((color) => {
			expect(hexColorRegex.test(color)).toBe(true);
		});
	});
});

import { stringAvatar, stringToColor } from '@/helpers/avatarColor';

describe('Helper function getStringAvatar', () => {
	it('Should return string of first letter of lastname and firstname', () => {
		expect(stringAvatar('Andrew Tate').children).toBe('AT');
		expect(stringAvatar('Andrew Tate  Test').children).toBe('AT');
	});
});

describe('stringToColor', () => {
	it('should generate a color from a string', () => {
		const result = stringToColor('hello');
		// Expected result based on the provided string and algorithm
		const expectedColor = '#ff8f1f';
		expect(result).toEqual(expectedColor);
	});

	it('should generate different colors for different input strings', () => {
		const color1 = stringToColor('string1');
		const color2 = stringToColor('string2');
		const color3 = stringToColor('string3');

		// Ensure that the generated colors are not the same
		expect(color1).not.toEqual(color2);
		expect(color2).not.toEqual(color3);
		expect(color3).not.toEqual(color1);
	});

	it('should handle empty input strings', () => {
		const result = stringToColor('');
		// Expected result for empty string
		const expectedColor = '#ffffff';
		expect(result).toEqual(expectedColor);
	});
});

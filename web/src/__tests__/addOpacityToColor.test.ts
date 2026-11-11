import { addOpacityToColor } from '../helpers/addOpacityToColor';

describe('Helper function addOpacityToColor', () => {
	it('Should convert opacity to hex value correctly', () => {
		const color1 = addOpacityToColor('ffffff', 0.3);
		const color2 = addOpacityToColor('ffffff', 0);
		const color3 = addOpacityToColor('ffffff', 1);

		expect(color1).toBe('ffffff4d');
		expect(color2).toBe('ffffff00');
		expect(color3).toBe('ffffffff');
	});

	it('Should return color without opacity if opacity less than 0', () => {
		const color = addOpacityToColor('ff0000', -1);
		expect(color).toBe(color);
	});

	it('Should return color without opacity if opacity greater than 1', () => {
		const color = addOpacityToColor('ff0000', 2);
		expect(color).toBe(color);
	});
});

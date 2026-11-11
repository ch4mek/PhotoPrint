export function getRandomColor(radix = 16, substrMin = 2, substrMax = 8) {
	return `#${`${Math.random().toString(radix)}000000`
		.substring(substrMin, substrMax)
		.toUpperCase()}`;
}

export const randomColorsArray = (randomColorsAmount: number) =>
	new Array(randomColorsAmount).fill('').map(() => getRandomColor());

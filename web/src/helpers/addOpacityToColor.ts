export const addOpacityToColor = (color: string, opacity: number) => {
	if (opacity > 1 || opacity < 0) {
		return color;
	}

	const opacityHex = Math.round(opacity * 255)
		.toString(16)
		.padStart(2, '0');

	return `${color}${opacityHex}`;
};

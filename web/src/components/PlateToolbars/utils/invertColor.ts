import { InvertColorPalettesProps } from '@/components/PlateToolbars/utils/types';

export function invertColor(
	color: string,
	palettes: InvertColorPalettesProps,
	isDark: boolean
): string {
	const fromPalette = isDark ? palettes.light : palettes.dark;
	const toPalette = isDark ? palettes.dark : palettes.light;

	const normalizedColor = color.toUpperCase();
	const index = fromPalette.findIndex(
		(c) => c.toUpperCase() === normalizedColor
	);

	if (index !== -1) {
		return toPalette[index];
	}

	return color;
}

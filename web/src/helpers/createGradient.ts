import { addOpacityToColor } from '@/helpers/addOpacityToColor';

export const createGradient = (
	mainColor: string,
	overlayColor: string,
	opacity: number
) =>
	`linear-gradient(0deg, ${addOpacityToColor(
		overlayColor,
		opacity
	)}, ${addOpacityToColor(overlayColor, opacity)}), 
		linear-gradient(0deg, ${mainColor}, ${mainColor})`;

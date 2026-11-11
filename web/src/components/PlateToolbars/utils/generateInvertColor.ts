import { invertColor } from '@/components/PlateToolbars/utils/invertColor';
import {
	darkBackgroundColorPalette,
	darkTextColorPalette,
	lightBackgroundColorPalette,
	lightTextColorPalette,
} from '@/constants/plateEditor';

export const generateInvertTextColor = (color: string, isDarkMode: boolean) =>
	invertColor(
		color,
		{
			light: lightTextColorPalette,
			dark: darkTextColorPalette,
		},
		isDarkMode
	);

export const generateInvertBackgroundColor = (
	color: string,
	isDarkMode: boolean
) =>
	invertColor(
		color,
		{
			light: lightBackgroundColorPalette,
			dark: darkBackgroundColorPalette,
		},
		isDarkMode
	);

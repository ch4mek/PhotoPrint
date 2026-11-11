import { HeaderStyles } from '@/types/helpers';

export const getHeaderStylesWithBackground = <T extends HeaderStyles>(
	baseStyles: T,
	background?: string
) => ({
	...baseStyles,
	background,
});

import { GetPopUpStylesConfigProps } from '../types';

import {
	POPUP_BUTTONS_CONTAINER_HEIGHT,
	popUpBreakpoints,
	popUpSizes,
} from '@/constants/popUp';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

export const getPopUpStylesConfig = ({
	theme,
	windowHeight,
	windowWidth,
}: GetPopUpStylesConfigProps) => ({
	container: {
		color: theme.palette['text-white'],
		width: popUpSizes.m,

		[theme.breakpoints.down(popUpBreakpoints.tabletXS)]: {
			width: popUpSizes.s,
		},
	},
	content: {
		...getAdaptivePopUpContentStylesConfig({
			theme,
			windowHeight: windowHeight - POPUP_BUTTONS_CONTAINER_HEIGHT,
			windowWidth,
		}),

		[theme.breakpoints.down(popUpBreakpoints.tabletXS)]: {
			...getAdaptivePopUpContentStylesConfig({
				theme,
				windowHeight: windowHeight - POPUP_BUTTONS_CONTAINER_HEIGHT,
			}),
		},
	},
	button: {
		flexWrap: 'wrap' as const,
	},
});

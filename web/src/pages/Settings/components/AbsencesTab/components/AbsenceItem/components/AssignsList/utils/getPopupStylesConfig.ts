import { Theme } from '@emotion/react';
import { spaces } from '@mms/mms-ui-library/theme';

import { popUpSizes } from '@/constants/popUp';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

const VERTICAL_PADDINGS = spaces.l * 2;

export const getPopupStylesConfig = (
	theme: Theme,
	windowHeight: number,
	contentHeight: number
) => ({
	container: {
		width: popUpSizes.s,
	},
	content: {
		...getAdaptivePopUpContentStylesConfig({
			theme,
			hasContols: false,
			hasTabs: false,
			windowHeight,
		}),
		height: contentHeight + VERTICAL_PADDINGS,
	},
});

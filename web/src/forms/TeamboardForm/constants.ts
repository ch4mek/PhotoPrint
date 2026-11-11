import { Theme } from '@emotion/react';

import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

export const getSpecializationPopupStyles = (
	theme: Theme,
	windowHeight: number
) => ({
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		windowHeight,
		hasContols: true,
	}),
});

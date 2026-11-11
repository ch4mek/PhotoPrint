import { useTheme } from '@emotion/react';
import { useMemo } from 'react';

import { RATE_HISTORY_ITEM_FORM_POPUP_MAX_WIDTH } from '../constants';

import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';
import { useResize } from '@/hooks/useResize';

export const usePopupStylesConfig = () => {
	const theme = useTheme();
	const [, windowHeight] = useResize();

	return useMemo(
		() => ({
			container: {
				width: '100%',
				maxWidth: RATE_HISTORY_ITEM_FORM_POPUP_MAX_WIDTH,
			},
			content: getAdaptivePopUpContentStylesConfig({
				theme,
				hasContols: true,
				windowHeight,
			}),
		}),
		[theme, windowHeight]
	);
};

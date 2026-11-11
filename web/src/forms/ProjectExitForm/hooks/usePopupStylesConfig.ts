import { useTheme } from '@emotion/react';
import { useMemo } from 'react';

import { getPopUpStylesConfig } from '../utils';

import { useResize } from '@/hooks/useResize';

export const usePopupStylesConfig = () => {
	const theme = useTheme();
	const [windowWidth, windowHeight] = useResize();

	const popUpStylesConfig = useMemo(
		() =>
			getPopUpStylesConfig({
				theme,
				windowWidth,
				windowHeight,
			}),
		[theme, windowWidth, windowHeight]
	);

	return { popUpStylesConfig };
};

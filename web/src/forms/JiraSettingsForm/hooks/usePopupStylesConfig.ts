import { useTheme } from '@emotion/react';
import { useMemo } from 'react';

import { jiraPopUpStylesConfig } from '@/constants/jira';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';
import { useResize } from '@/hooks/useResize';

export const usePopupStylesConfig = () => {
	const theme = useTheme();
	const [windowWidth, windowHeight] = useResize();

	const popUpStylesConfig = useMemo(
		() => ({
			content: getAdaptivePopUpContentStylesConfig({
				theme,
				windowHeight,
				windowWidth,
			}),
			...jiraPopUpStylesConfig,
		}),
		[theme, windowHeight, windowWidth]
	);

	return popUpStylesConfig;
};

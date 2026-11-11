import styled from '@emotion/styled';
import { ThemeMode } from '@mms/mms-ui-library/theme';

import darkThemeLayoutBackground from './darkThemeLayout.svg';
import layoutBackground from './layout.svg';

const MINSK_SPACE_F_LAYOUT_HEIGHT = 797;
const MINSK_SPACE_F_LAYOUT_WIDTH = 925;

export const MinskSpaceFLayoutWrapper = styled('div')(
	({ theme }) =>
		({
			position: 'relative',
			margin: 'auto',
			height: MINSK_SPACE_F_LAYOUT_HEIGHT,
			width: MINSK_SPACE_F_LAYOUT_WIDTH,
			backgroundImage: `url("${
				theme.mode === ThemeMode.DARK
					? darkThemeLayoutBackground
					: layoutBackground
			}")`,
		} as const)
);

import styled from '@emotion/styled';
import { ThemeMode } from '@mms/mms-ui-library/theme';

import darkThemelayoutBackground from './darkThemeLayout.svg';
import layoutBackground from './layout.svg';

const MINSK_SPACE_D_LAYOUT_HEIGHT = 396;
const MINSK_SPACE_D_LAYOUT_WIDTH = 478;

export const MinskSpaceDLayoutWrapper = styled('div')(
	({ theme }) =>
		({
			position: 'relative',
			height: MINSK_SPACE_D_LAYOUT_HEIGHT,
			width: MINSK_SPACE_D_LAYOUT_WIDTH,
			margin: 'auto',
			backgroundImage: `url("${
				theme.mode === ThemeMode.DARK
					? darkThemelayoutBackground
					: layoutBackground
			}")`,
		} as const)
);

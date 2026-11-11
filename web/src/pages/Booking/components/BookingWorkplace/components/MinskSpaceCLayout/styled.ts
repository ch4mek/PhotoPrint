import styled from '@emotion/styled';
import { ThemeMode } from '@mms/mms-ui-library/theme';

import darkThemelayoutBackground from './darkThemeLayout.svg';
import layoutBackground from './layout.svg';

const MINSK_SPACE_C_LAYOUT_HEIGHT = 539;
const MINSK_SPACE_C_LAYOUT_WIDTH = 528;

export const MinskSpaceCLayoutWrapper = styled('div')(
	({ theme }) =>
		({
			position: 'relative',
			height: MINSK_SPACE_C_LAYOUT_HEIGHT,
			width: MINSK_SPACE_C_LAYOUT_WIDTH,
			margin: 'auto',
			backgroundImage: `url("${
				theme.mode === ThemeMode.DARK
					? darkThemelayoutBackground
					: layoutBackground
			}")`,
		} as const)
);

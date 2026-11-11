import styled from '@emotion/styled';

import { HEADER_HEIGHT } from '@/layouts/main/constants';

export const CHAT_HEIGHT = 459;
const CHAT_WIDTH = 350;
const BORDER_WIDTH = 1;

export const ChatWrapper = styled('div')<{ windowHeight: number }>(
	({ windowHeight, theme }) => ({
		width: CHAT_WIDTH,
		height: CHAT_HEIGHT,

		display: 'flex',
		flexDirection: 'column',

		[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
			width: '100%',
			height: `calc(${windowHeight}px - ${HEADER_HEIGHT}px - ${BORDER_WIDTH}px)`,
		},
	})
);

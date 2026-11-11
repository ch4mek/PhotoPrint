import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { breakpoints } from '@mms/mms-ui-library/theme';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { ChatRole } from '@/types/chatWidget';

import { messageColorMap, messageTextColorMap } from './constants';

const MAX_MESSAGE_WIDTH_S = 278;
const MAX_MESSAGE_WIDTH_M = '80%';
const LOADING_MESSAGE_HEIGHT = 11;

export const MessageWrapper = styled('li')<{ isOwnMessage: boolean }>(
	({ isOwnMessage }) => ({
		display: 'flex',
		justifyContent: isOwnMessage ? 'flex-end' : 'flex-start',
	})
);

export const MessageItem = styled('div')<{ role: ChatRole }>(
	({ theme, role }) => ({
		display: 'flex',
		flexDirection: 'column',
		maxWidth: MAX_MESSAGE_WIDTH_S,
		padding: theme.spaces.s,
		borderRadius: BorderRadiuses.m,
		background: messageColorMap(theme)[role][theme.mode],

		[theme.breakpoints.down(breakpoints.desktopXS)]: {
			maxWidth: MAX_MESSAGE_WIDTH_M,
		},
	})
);

export const LoadingMessageWrapper = styled('div')({
	display: 'flex',
	alignItems: 'flex-end',
	height: LOADING_MESSAGE_HEIGHT,
});

export const MessageContent = styled(Typography)<{
	role: ChatRole;
	mainColor: string;
}>(({ theme, role, mainColor }) => ({
	color: messageTextColorMap(theme, mainColor)[role][theme.mode],
	whiteSpace: 'pre-wrap',
	wordWrap: 'break-word',
}));

export const MessageTime = styled('time')<{
	role: ChatRole;
	mainColor: string;
}>(({ theme, role, mainColor }) => ({
	display: 'flex',
	justifyContent: 'flex-end',
	color: messageTextColorMap(theme, mainColor)[role][theme.mode],
}));

import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { IconSize } from '@/constants/iconSizes';

const CHAT_STATUS_COLOR = '#18DA25';
const BADGE_POSITION = 2;

export const ChatAvatarWrapper = styled('div')(({ theme }) => ({
	position: 'relative',
	width: 'fit-content',

	'&::after': {
		content: "''",
		position: 'absolute',
		borderRadius: BorderRadiuses.half,
		height: IconSize.XS,
		width: IconSize.XS,
		border: `1px solid ${theme.palette['background-white']}`,
		background: CHAT_STATUS_COLOR,
		bottom: BADGE_POSITION,
		right: BADGE_POSITION,
	},
}));

export const ChatHeaderInfoWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	flex: 1,
}));

export const ChatHeaderInfoDetails = styled('div')({
	display: 'flex',
	flexDirection: 'column',
	flex: 1,
});

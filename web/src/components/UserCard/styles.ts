import styled from '@emotion/styled';
import { Avatar } from '@mms/mms-ui-library/components/Avatar';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { Link } from 'react-router-dom';

const NUMBER_OF_LINES = 1;

export const UserCardWrapper = styled('article')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
	flex: 1,
	width: '100%',
	minWidth: 0,
}));

export const UserCardInformation = styled('div')<{ avatarWidth: number }>(
	({ theme, avatarWidth }) => ({
		display: 'flex',
		flexDirection: 'column',
		width: `calc(100% - ${avatarWidth + theme.spaces.s}px)`,
		minWidth: 0,
	})
);

export const UserCardAvatar = styled(Avatar)(({ theme }) => ({
	...theme.typography.body['s-600'],
}));

export const InformationTypography = styled(Typography)({
	display: '-webkit-box',
	textOverflow: 'ellipsis',
	overflow: 'hidden',
	lineClamp: `${NUMBER_OF_LINES}`,
	'-webkit-line-clamp': `${NUMBER_OF_LINES}`,
	'-webkit-box-orient': 'vertical',
	minWidth: 0,
});

export const UserCardLink = styled(Link)({
	textDecoration: 'none',
	color: 'inherit',
	'&:hover': {
		textDecoration: 'underline',
	},
});

export const UserCardAvatarLink = styled(Link)({
	display: 'inline-flex',
	alignItems: 'center',
	justifyContent: 'center',
	flexShrink: 0,
	borderRadius: '50%',
	lineHeight: 0,
	textDecoration: 'none',
	color: 'inherit',
});

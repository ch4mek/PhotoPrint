import styled from '@emotion/styled';
import { Avatar } from '@mms/mms-ui-library/components/Avatar';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { lightTheme } from '@mms/mms-ui-library/theme';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { HOME_MOBILE_BREAKPOINT } from '@/pages/Home/HomeContent/constants';

const NUMBER_OF_LINES = 2;
const MOBILE_NUMBER_OF_LINES = 1;

export const BirthdayCardWrapper = styled('div')<{ backgroundImage: string }>(
	({ theme, backgroundImage }) => ({
		display: 'flex',
		flexDirection: 'column',
		alignItems: 'center',
		justifyContent: 'center',
		gap: theme.spaces.s,

		padding: `${theme.spaces.m}px ${theme.spaces.s}px`,
		color: lightTheme.palette['text-black'],
		borderRadius: BorderRadiuses.xs2,

		backgroundImage: `url(${backgroundImage})`,
		backgroundSize: 'cover',
		backgroundRepeat: 'no-repeat',
		backgroundPosition: '100%',

		[theme.breakpoints.down(HOME_MOBILE_BREAKPOINT)]: {
			flexDirection: 'row',
			justifyContent: 'start',
			padding: theme.spaces.s,
		},
	})
);

export const BirthdayAvatar = styled(Avatar)(({ theme }) => ({
	...theme.typography.body['s-600'],
}));

export const BirthdayCardInformation = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,
	alignItems: 'center',
	justifyContent: 'center',
	flex: 1,

	[theme.breakpoints.down(HOME_MOBILE_BREAKPOINT)]: {
		alignItems: 'start',
	},
}));

export const InformationTypography = styled(Typography)(({ theme }) => ({
	display: '-webkit-box',
	overflowWrap: 'break-word',
	wordBreak: 'break-word',
	whiteSpace: 'normal',

	lineClamp: `${NUMBER_OF_LINES}`,
	WebkitLineClamp: `${NUMBER_OF_LINES}`,
	WebkitBoxOrient: 'vertical',
	textOverflow: 'ellipsis',
	overflow: 'hidden',

	[theme.breakpoints.down(HOME_MOBILE_BREAKPOINT)]: {
		textAlign: 'start',
		lineClamp: `${MOBILE_NUMBER_OF_LINES}`,
		WebkitLineClamp: `${MOBILE_NUMBER_OF_LINES}`,
	},
}));

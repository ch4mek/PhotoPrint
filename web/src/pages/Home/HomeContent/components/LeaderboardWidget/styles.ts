import styled from '@emotion/styled';
import { Avatar } from '@mms/mms-ui-library/components/Avatar';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';

import { topResultBackgroundColors } from './constants';
import { LeaderboardColumnProps, LeaderboardHighlightProps } from './types';

export const LeaderboardTrophyIconWrapper = styled('span')(({ theme }) => ({
	display: 'flex',
	color: theme.palette['text-accent'],
}));

export const LeaderboardList = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: 'repeat(2, minmax(0, 1fr))',
	gap: theme.spaces.s,
	width: '100%',
}));

export const LeaderboardColumn = styled('ol')<LeaderboardColumnProps>(
	({ theme }) => ({
		display: 'flex',
		flexDirection: 'column',
		gap: theme.spaces.s,
		margin: 0,
		padding: 0,
		listStyle: 'none',
	})
);

export const LeaderboardCardWrapper = styled('li')<LeaderboardHighlightProps>(
	({ theme, isTopResult }) => ({
		display: 'flex',
		alignItems: 'center',
		gap: theme.spaces.s,
		minWidth: 0,
		padding: `12px ${theme.spaces.s}px`,
		border: `1px solid ${theme.palette['graphics-divider-1']}`,
		borderRadius: BorderRadiuses.m,
		backgroundColor: isTopResult
			? topResultBackgroundColors[theme.mode]
			: theme.palette['background-base-1'],
	})
);

export const LeaderboardPlace = styled(Typography)<LeaderboardHighlightProps>(
	({ theme, isTopResult }) => ({
		flex: '0 0 24px',
		color: isTopResult
			? theme.palette['text-accent']
			: theme.palette['text-gray'],
		textAlign: 'center',
	})
);

export const LeaderboardAvatar = styled(Avatar)(({ theme }) => ({
	...theme.typography.body['s-600'],
	flexShrink: 0,
}));

export const LeaderboardUserInformation = styled('div')({
	display: 'flex',
	gap: 2,
	flex: 1,
	flexDirection: 'column',
	minWidth: 0,
});

export const LeaderboardUserDetails = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'flex-end',
	justifyContent: 'space-between',
	gap: theme.spaces.xs,
	minWidth: 0,
}));

export const LeaderboardUserText = styled(Typography)(({ theme }) => ({
	display: 'block',
	width: '100%',
	minWidth: 0,
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
	color: theme.palette['text-black'],
}));

export const LeaderboardDepartmentText = styled(LeaderboardUserText)(
	({ theme }) => ({
		color: theme.palette['text-gray'],
	})
);

export const LeaderboardScore = styled(Typography)(({ theme }) => ({
	marginLeft: 'auto',
	flexShrink: 0,
	color: theme.palette['text-gray'],
}));

export const LeaderboardEmptyMessage = styled(Typography)(({ theme }) => ({
	margin: 'auto',
	color: theme.palette['text-gray'],
}));

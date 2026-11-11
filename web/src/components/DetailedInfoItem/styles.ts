import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const INFO_ITEM_TABLET_WIDTH = '235px';
const INFO_ITEM_TABLET_MIN_WIDTH = '152px';
const DETAILED_INFO_TIME_LEFT_INDENTATION = 14;
const NAME_ROW_HEIGHT = 18;

export const DetailedInfoItemWrapper = styled('li')<{ isActive: boolean }>(
	({ theme, isActive }) => ({
		padding: `${theme.spaces.xs}px ${theme.spaces.s}px`,
		borderRadius: BorderRadiuses.m,
		border: `1px solid ${
			isActive
				? theme.palette['graphics-input-stroke']
				: theme.palette['graphics-divider-1']
		}`,
		background: isActive ? theme.palette['hover-base-2'] : 'inherit',
		cursor: 'pointer',
		...theme.typography.body['m-600'],

		[theme.breakpoints.down(theme.breakpoints.tablet)]: {
			flexShrink: 0,
			width: INFO_ITEM_TABLET_WIDTH,
		},
		[theme.breakpoints.down(theme.breakpoints.tablet)]: {
			width: '100%',
		},
		[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
			minWidth: INFO_ITEM_TABLET_MIN_WIDTH,
		},
	})
);

export const DetailedInfoNameWrapper = styled('div')({
	position: 'relative',
	display: 'flex',
	alignItems: 'center',
	height: `${NAME_ROW_HEIGHT}px`,
});

export const DetailedInfoName = styled(Typography)(({ theme }) => ({
	position: 'absolute',
	top: 0,
	right: 0,
	bottom: 0,
	left: DETAILED_INFO_TIME_LEFT_INDENTATION,
	width: '85%',
	...theme.typography.body['m-600'],
	wordBreak: 'break-all',
	maxWidth: '100%',
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	whiteSpace: 'nowrap',
}));

export const DetailedInfoTime = styled(Typography)(({ theme }) => ({
	marginLeft: DETAILED_INFO_TIME_LEFT_INDENTATION,
	...theme.typography.body['m-400'],
}));

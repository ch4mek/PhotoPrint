import styled from '@emotion/styled';

import { TIME_LABELS_HEIGHT, TABLE_HEADER_HEIGHT } from '../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const TIMELINE_SECTIONS_COLUMN_WIDTH = 147;

export const TimelineSectionsColumnWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: TIME_LABELS_HEIGHT,
	width: TIMELINE_SECTIONS_COLUMN_WIDTH,
	minWidth: TIMELINE_SECTIONS_COLUMN_WIDTH,
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	borderRadius: `${BorderRadiuses.xs2}px 0 0 ${BorderRadiuses.xs2}px`,
	borderRight: 0,
}));

export const TimelineSectionsColumnTitleWrapper = styled('div')(
	({ theme }) => ({
		height: TABLE_HEADER_HEIGHT,
		borderBottom: `1px solid ${theme.palette['graphics-input-stroke']}`,
		paddingLeft: theme.spaces.s,
		display: 'flex',
		alignItems: 'center',
		boxSizing: 'border-box',
	})
);

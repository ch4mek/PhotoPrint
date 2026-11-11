import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { CALENDAR_GRID_ROW_HEIGHT } from '../AbsenceCalendarGrid/constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const CALENDAR_TIMELINE_ITEM_PADDING_Y = 6;
const CALENDAR_TIMELINE_ITEM_HEIGHT = 32;

export const CalendarTimelineItemContainer = styled('div')<{
	level: number;
}>(({ level }) => ({
	cursor: 'pointer',
	position: 'absolute',
	top: CALENDAR_GRID_ROW_HEIGHT * level,
	left: 0,
	zIndex: 1,
}));

export const CalendarTimelineWrapper = styled('div')<{
	background: string;
	borderColor: string;
	width: number;
	isStartPart: boolean;
	isEndPart: boolean;
}>(({ theme, background, borderColor, width, isStartPart, isEndPart }) => {
	const radius = BorderRadiuses.m;

	const leftRadius = isEndPart ? 0 : radius;
	const rightRadius = isStartPart ? 0 : radius;

	const horizontalPadding =
		theme.spaces.xs * (2 - Number(isStartPart || isEndPart));

	return {
		padding: `${CALENDAR_TIMELINE_ITEM_PADDING_Y}px ${theme.spaces.xs}px`,
		outline: `1px solid ${borderColor}`,
		overflow: 'hidden',
		background,

		width: `calc(${width}px - ${horizontalPadding}px)`,
		minHeight: CALENDAR_TIMELINE_ITEM_HEIGHT,

		margin: theme.spaces.xs,
		marginLeft: isEndPart ? 0 : theme.spaces.xs,
		marginRight: isStartPart ? 0 : theme.spaces.xs,

		borderRadius: `${leftRadius}px ${rightRadius}px ${rightRadius}px ${leftRadius}px`,
	};
});

export const CalendarTimelineContent = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.xs,
	minWidth: 0,
}));

export const CalendarTimelineTypography = styled(Typography)<{
	color: string;
}>(({ theme, color }) => ({
	...theme.typography.body['m-400'],
	color,
}));

export const IconWrapper = styled('div')({
	flexShrink: 0,
	display: 'flex',
});

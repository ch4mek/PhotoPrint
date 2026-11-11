import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { Skeleton } from '@mui/material';

import { CELL_HEIGHT } from '../../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const TimelineSectionWrapper = styled('div')<{
	isSelected: boolean;
	isDisabled: boolean;
	timelineHeight: number;
}>(({ theme, isSelected, isDisabled, timelineHeight }) => ({
	position: 'relative',
	display: 'flex',
	alignItems: 'center',
	cursor: isDisabled ? 'default' : 'pointer',
	minHeight: timelineHeight,
	backgroundColor: 'transparent',
	borderTop: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderBottomLeftRadius: BorderRadiuses.xs2,
	paddingLeft: theme.spaces.s,
	paddingRight: theme.spaces.s,
	color: isSelected
		? theme.palette['text-accent']
		: theme.palette['text-black'],
}));

export const SectionEquipment = styled('div')(({ theme }) => ({
	display: 'flex',
	flexWrap: 'wrap',
	gap: theme.spaces.xs,
}));

export const TimelineSectionContent = styled('div')(({ theme }) => ({
	paddingTop: theme.spaces.s,
	paddingBottom: theme.spaces.s,
}));

export const TimelineSectionSkeleton = styled(Skeleton)({
	height: CELL_HEIGHT,
	width: '100%',
});

export const MeetingRoomNameTypography = styled(Typography)({
	whiteSpace: 'wrap',
});

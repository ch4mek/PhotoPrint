import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { Row } from '../styles';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { IconSize } from '@/constants/iconSizes';

import { BOOKED_LABEL_HEIGHT } from './constants';

export const BookedLabelContainer = styled('div')<{
	width: number;
	posX: number;
}>(({ theme, width, posX }) => ({
	position: 'absolute',
	height: '100%',
	display: 'flex',
	alignItems: 'center',
	marginLeft: theme.spaces.xs,
	left: posX,
	width,
}));

export const BookedLabelWrapper = styled('div')<{
	isDisabled: boolean;
	hideRightPadding: boolean;
	isClippedOnRight: boolean;
}>(({ theme, isDisabled, hideRightPadding, isClippedOnRight }) => ({
	position: 'relative',
	display: 'flex',
	flexDirection: 'column',
	justifyContent: 'space-between',
	alignItems: 'flex-start',
	backgroundColor: theme.palette['background-primary-1'],
	height: BOOKED_LABEL_HEIGHT,
	width: '100%',
	padding: theme.spaces.xs,
	borderLeft: `${BorderRadiuses.xs2}px solid ${theme.palette['graphics-primary-2']}`,
	borderRadius: BorderRadiuses.xs2,
	...(isClippedOnRight && {
		borderTopRightRadius: 0,
		borderBottomRightRadius: 0,
	}),
	cursor: isDisabled ? 'default' : 'pointer',
	...(hideRightPadding && { paddingRight: 0 }),
}));

export const FixedTypography = styled(Typography)({
	maxWidth: '100%',
	textWrap: 'nowrap',
	overflow: 'hidden',
});

export const BookedTimePeriod = styled(Row)({
	width: '100%',
	justifyContent: 'space-between',
	alignItems: 'flex-end',
	overflow: 'hidden',
});

export const BookedTimePeriodRow = styled(Row)({
	display: 'flex',
	flex: 1,
	overflow: 'hidden',
});

export const ClockIconWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'center',
	alignItems: 'center',
	color: theme.palette['graphics-primary-2'],
	width: IconSize.S,
	height: IconSize.S,

	'& > svg': {
		width: IconSize.S,
		height: IconSize.S,
	},
}));

const quarterStyles = {
	position: 'absolute',
	bottom: 4,
	right: 0,
	display: 'flex',
	justifyContent: 'center',
	alignContent: 'center',
} as const;

const QUARTER_ICON_SIZE = 14;

export const LoopIconWrapper = styled('div')<{
	isQuarterPeriod: boolean;
}>(({ isQuarterPeriod, theme }) => {
	const iconSize = isQuarterPeriod ? QUARTER_ICON_SIZE : IconSize.MXS;

	return {
		width: iconSize,
		height: iconSize,
		color: theme.palette['graphics-primary-2'],
		...(isQuarterPeriod && quarterStyles),

		'& > svg': {
			width: iconSize,
			height: iconSize,
		},
	};
});

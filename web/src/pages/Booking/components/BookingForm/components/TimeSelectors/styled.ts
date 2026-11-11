import { Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { Tag } from '@mms/mms-ui-library/components/Tag';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { lightPalette } from '@mms/mms-ui-library/theme';

import { GREEN_TAG_COLOR, RED_TAG_COLOR } from '../../../../../../theme';
import { FieldHeaderWrapper } from '../FieldHeaderWrapper';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import {
	DEFAULT_DISABLED_INPUT_COMPONENT_OPACITY,
	DEFAULT_OPACITY,
} from '@/constants/opacities';
import { BOOKING_DEFAULT_ICON_SIZE } from '@/pages/Booking/constants';

const TAG_HEIGHT = 18;
const DIFFERENCE_CONTAINER_WIDTH = 40;

const getTagColor = (theme: Theme, isAvailable: boolean) =>
	`${isAvailable ? GREEN_TAG_COLOR : RED_TAG_COLOR}`;

export const TimeSelectorsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column' as const,
	gap: theme.spaces.m,
	width: '100%',
}));

export const TimeSelectorsContentWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	gap: theme.spaces.s,
}));

export const TimeSelectorsHeader = styled(FieldHeaderWrapper)(({ theme }) => ({
	color: theme.palette['text-black'],
}));

export const IconWrapper = styled('div')(() => ({
	display: 'flex',
	justifyContent: 'center',
	alignItems: 'center',
	width: BOOKING_DEFAULT_ICON_SIZE,
	height: BOOKING_DEFAULT_ICON_SIZE,

	'& > svg': {
		width: BOOKING_DEFAULT_ICON_SIZE,
		height: BOOKING_DEFAULT_ICON_SIZE,
	},
}));

export const TimeRangeDifferenceContainer = styled('div')(
	() =>
		({
			display: 'flex',
			flexDirection: 'column',
			justifyContent: 'center',
			width: DIFFERENCE_CONTAINER_WIDTH,
			flexShrink: 0,
		} as const)
);

export const TimeRangeDifferenceTypography = styled(Typography)<{
	disabled?: boolean;
}>(({ theme, disabled }) => ({
	lineHeight: theme.typography.body['s-600'].lineHeight,
	color: theme.palette['text-black'],
	opacity: disabled
		? DEFAULT_DISABLED_INPUT_COMPONENT_OPACITY
		: DEFAULT_OPACITY,
}));

export const AvailabilityTag = styled(Tag)<{ isAvailable: boolean }>(
	({ theme, isAvailable }) => ({
		height: TAG_HEIGHT,
		padding: `0 ${theme.spaces.s}px`,
		border: 'none',
		marginLeft: theme.spaces.m,
		color: lightPalette['text-black'],
		borderRadius: BorderRadiuses.xs2,

		...theme.typography.body['s-400'],

		'&, &:hover': {
			backgroundColor: getTagColor(theme, isAvailable),
		},
	})
);

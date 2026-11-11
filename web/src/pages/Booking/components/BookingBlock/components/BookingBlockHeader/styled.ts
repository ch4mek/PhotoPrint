import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { lightPalette } from '@mms/mms-ui-library/theme';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { PLAIN_TEXT_FONT_SIZE } from '@/constants/commonStyles';

const HEADER_HEIGHT = 40;
const HEADER_HORIZONTAL_PADDING = 20;

export const BookingsHeaderWrapper = styled('div')(({ theme }) => ({
	borderTopLeftRadius: BorderRadiuses.m,
	borderTopRightRadius: BorderRadiuses.m,
	background: theme.palette['background-primary-3'],
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	color: lightPalette['text-white'],
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	height: HEADER_HEIGHT,
	flexDirection: 'row',
	padding: `0px ${HEADER_HORIZONTAL_PADDING}px`,
	flexShrink: 0,
}));

export const BookingsTitle = styled(Typography)(({ theme }) => ({
	color: lightPalette['text-white'],

	[theme.breakpoints.down(theme.breakpoints.tablet)]: {
		fontSize: PLAIN_TEXT_FONT_SIZE,
	},
}));

export const BookingsHeaderRightMenuWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.m,
	'& > button': {
		padding: 0,
		display: 'flex',
		justifyContent: 'center',
		alignItems: 'center',
	},
}));

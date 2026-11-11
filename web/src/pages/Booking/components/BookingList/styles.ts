import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BOOKING_BLOCK_HEADER_HEIGHT } from '../../constants';

import { COLOR_MAP } from '@/constants/booking';
import { BookingListItemProps } from '@/pages/Booking/components/BookingList/types';

import { BOOKING_LIST_TYPOGRAPHY_CLASS } from './constants';

const CLOSE_ICON_MAX_WIDTH = 56;
const DIVIDER_HEIGHT = 1;
const LIST_ITEM_WITH_ICON_SIZE = 20;
const MAX_WITH_GUEST_NAME = 55;

export const ListWrapper = styled('div')({
	width: '100%',
	boxSizing: 'border-box',
	height: `calc(100% - ${BOOKING_BLOCK_HEADER_HEIGHT}px)`,
	overflow: 'hidden',
});

export const BookingListItem = styled('div')<BookingListItemProps>(
	({ theme, type, isNeedUnderline = false }) => ({
		position: 'relative',
		boxSizing: 'border-box',
		display: 'flex',
		justifyContent: 'space-between',
		padding: `${theme.spaces.s}px`,
		paddingLeft: `${theme.spaces.m}px`,
		paddingTop: `${theme.spaces.xs}px`,
		transition: 'all 0.2s ease-in-out',
		cursor: 'default',

		'&::before': {
			content: '" "',
			position: 'absolute',
			height: '100%',
			top: 0,
			left: 0,
			backgroundColor:
				theme.palette[
					COLOR_MAP[
						type as keyof typeof COLOR_MAP
					] as keyof typeof theme.palette
				],
			width: theme.spaces.s,
		},
		'&:hover': {
			backgroundColor: theme.palette['hover-base-2'],
		},
		[`&:hover .${BOOKING_LIST_TYPOGRAPHY_CLASS}`]: {
			textDecoration: isNeedUnderline ? 'underline' : 'none',
			cursor: isNeedUnderline ? 'pointer' : 'default',
		},
	})
);

export const ListItemContent = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,
	maxWidth: '90%',
	marginTop: theme.spaces.xs,
}));

export const BookingListTypography = styled(Typography)<{
	isBookedBy?: boolean;
}>(({ isBookedBy = false }) => ({
	display: 'inlineBlock',
	verticalAlign: 'middle',
	lineHeight: 'normal',
	overflowWrap: 'anywhere',
	maxWidth: isBookedBy ? `${MAX_WITH_GUEST_NAME}%` : 'auto',
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
}));

export const BookingListForGuest = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xs,
	alignItems: 'flex-end',
}));

export const BookedCreatorTypography = styled(Typography)(({ theme }) => ({
	color: theme.palette['graphics-primary-2'],
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
	flex: 1,
}));

export const BookingUserName = styled('span')({
	display: 'inline',
});

export const IconWrapper = styled('div')(({ theme }) => ({
	color: theme.palette['graphics-primary-2'],
	width: LIST_ITEM_WITH_ICON_SIZE,
	height: LIST_ITEM_WITH_ICON_SIZE,
	display: 'flex',
	justifyContent: 'center',
	alignItems: 'center',
}));

export const IconCloseWrapper = styled('div')(() => ({
	display: 'flex',
	maxWidth: CLOSE_ICON_MAX_WIDTH,
	justifyContent: 'center',
	alignItems: 'flex-start',
}));

export const Divider = styled('div')(({ theme }) => ({
	width: '100%',
	height: DIVIDER_HEIGHT,
	background: theme.palette['graphics-divider-2'],

	'&:last-child': {
		display: 'none',
	},
}));

export const ItemRowWithIcon = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xs,
	height: LIST_ITEM_WITH_ICON_SIZE,
}));

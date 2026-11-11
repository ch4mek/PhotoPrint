import styled from '@emotion/styled';
import {
	Scrollbar,
	getScrollbarStyles,
} from '@mms/mms-ui-library/components/Scrollbar';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';

import { ActiveOption } from './types';

const BORDER_WIDTH = 1;
const MODAL_WIDTH = 244;
const TYPOGRAPHY_WRAPPER_HEIGHT = 30;
const TEXT_OVERFLOW_WIDTH = 190;

export const CustomScroll = styled(Scrollbar)(({ theme }) => ({
	...getScrollbarStyles(theme, scrollbarWidths.s, 'vertical'),
}));

export const ModalWrapper = styled('div')(({ theme }) => ({
	width: MODAL_WIDTH,
	padding: theme.spaces.m,
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
	'> div:first-of-type': {
		height: theme.spaces.xl2,
	},
}));

export const ScrollbarWrapper = styled('div')(({ theme }) => ({
	marginLeft: -theme.spaces.m,
	marginRight: -(theme.spaces.m - BORDER_WIDTH),
}));

export const TypographyWrapper = styled('div')<ActiveOption>(
	({ theme, isActive }) => ({
		cursor: 'pointer',
		display: 'flex',
		alignItems: 'center',
		height: TYPOGRAPHY_WRAPPER_HEIGHT,
		marginLeft: theme.spaces.m,
		marginRight: theme.spaces.s + scrollbarWidths.s,
		padding: theme.spaces.s,
		borderRadius: BorderRadiuses.m,
		'&:hover': {
			backgroundColor: theme.palette['hover-base-2'],
		},
		backgroundColor: isActive
			? theme.palette['background-primary-1']
			: 'transparent',
	})
);

export const TypographyNoResult = styled(Typography)(({ theme }) => ({
	marginLeft: theme.spaces.m,
	marginRight: theme.spaces.s + scrollbarWidths.s,
	padding: theme.spaces.s,
	color: theme.palette['text-black'],
}));

export const TextOverflow = styled('div')(() => ({
	textOverflow: 'ellipsis',
	width: TEXT_OVERFLOW_WIDTH,
	overflow: 'hidden',
}));

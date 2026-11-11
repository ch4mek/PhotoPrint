import { Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { IconSize } from '@/constants/iconSizes';

const POPOVER_WIDTH = 250;

export const getCancelBookingPopoverStyles = (theme: Theme) => ({
	width: POPOVER_WIDTH,
	padding: theme.spaces.m,
	background: theme.palette['background-white'],
	borderRadius: BorderRadiuses.m,
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	boxShadow: theme.namedShadows.dropDownListWrapper,
});

export const CancelBookingPopoverWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
}));

export const CancelContentWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

export const ItemRowWithIcon = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xs,
}));

export const IconWrapper = styled('div')(({ theme }) => ({
	color: theme.palette['graphics-primary-2'],
	width: IconSize.MS,
	height: IconSize.MS,
	display: 'flex',
	justifyContent: 'center',
	alignItems: 'center',
}));

export const RowContentTypography = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-black'],
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
	flex: 1,
}));

export const ClickableRowContentTypography = styled(RowContentTypography)<{
	onClick?: VoidFunction;
}>(({ onClick }) => ({
	cursor: onClick ? 'pointer' : 'default',
	textDecoration: 'none',
	...(onClick && {
		'&:hover': {
			textDecoration: 'underline',
		},
	}),
}));

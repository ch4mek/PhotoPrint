import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { IconSize } from '@/constants/iconSizes';
import { MOBILE_VIEW_REQUEST_FORM_BREAKPOINT } from '@/forms/Absence/ViewAbsenceRequestForm/constants';

const DISABLE_ICON_OPACITY = 0.3;

export const ApproverItemWrapper = styled('li')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	padding: theme.spaces.s,
	gap: theme.spaces.s,
	flex: 1,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: BorderRadiuses.m,
	minWidth: 0,

	[theme.breakpoints.down(MOBILE_VIEW_REQUEST_FORM_BREAKPOINT)]: {
		flexDirection: 'column',
		alignItems: 'flex-end',
		gap: theme.spaces.xs,
	},
}));

export const ApproverInfo = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	gap: theme.spaces.s,
	flex: 1,
	width: '100%',
	minWidth: 0,
}));

export const IconWrapper = styled('div')<{
	color: string;
	isDisabled: boolean;
}>(({ color, isDisabled }) => ({
	width: IconSize.M,
	height: IconSize.M,
	color,

	...(isDisabled && {
		opacity: DISABLE_ICON_OPACITY,
	}),
}));

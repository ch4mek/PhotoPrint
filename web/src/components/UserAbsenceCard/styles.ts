import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';

import { MIN_ITEM_WIDHT } from './constants';

const ICON_WRAPPER_SIZE = 24;
const UNIT_TEXT_GRADIENT =
	'linear-gradient(265.88deg, #FFBC50 4.82%, #FA6358 100.87%)';

const amountDaysTypographyStyled = {
	fontSize: 56,
	fontWeight: 700,
	lineHeight: '67px',
};

export const AbsenceItemWrapper = styled('div')<{
	width?: string | number;
	backgroundColor?: string;
}>(({ width, backgroundColor }) => ({
	display: 'flex',
	flexDirection: 'column',
	borderRadius: BorderRadiuses.m,
	width: width ?? MIN_ITEM_WIDHT,
	position: 'relative',
	backgroundColor,
}));

export const AbsenceItemHeader = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
	justifyContent: 'space-between',
	padding: `${theme.spaces.s}px ${theme.spaces.s}px 0`,
}));

export const AbsenceItemHeaderContent = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
	minWidth: 0,
}));

export const AbsenceItemMain = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
	padding: `0 ${theme.spaces.s}px ${theme.spaces.s}px`,
}));

export const AbsenceItemMainInfoSection = styled('section')({
	display: 'flex',
	flexDirection: 'column',
});

export const AvailabilityTypography = styled(Typography)(({ theme }) => ({
	textAlign: 'center',
	color: theme.palette['text-gray'],
	...theme.typography.body['s-400'],
}));

export const AmountDaysTypography = styled(Typography)({
	textAlign: 'center',
	background: UNIT_TEXT_GRADIENT,
	WebkitBackgroundClip: 'text',
	WebkitTextFillColor: 'transparent',
	backgroundClip: 'text',
	...amountDaysTypographyStyled,
});

export const IconWrapper = styled('div')<{ backgroundColor?: string }>(
	({ backgroundColor }) => ({
		width: ICON_WRAPPER_SIZE,
		height: ICON_WRAPPER_SIZE,
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'center',
		flexShrink: 0,
		borderRadius: BorderRadiuses.xs2,
		background: backgroundColor,
	})
);

export const AbsenceTypeTypography = styled(Typography)(({ theme }) => ({
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
	color: theme.palette['text-gray'],
}));

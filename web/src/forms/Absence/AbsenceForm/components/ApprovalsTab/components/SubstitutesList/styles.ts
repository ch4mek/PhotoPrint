import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { MOBILE_L_BREAKPOINT } from '../../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const ARROW_ICON_HEIGHT = 18;
const ARROW_ICON_WIDTH = 11;
const SCOPE_ITEM_HEIGHT = 40;

export const ScopeItem = styled('div')(({ theme }) => ({
	padding: theme.spaces.s,
	height: SCOPE_ITEM_HEIGHT,
	borderRadius: BorderRadiuses.m,
	backgroundColor: theme.palette['background-base-2'],
	display: 'flex',
	alignItems: 'center',
	flexGrow: 4,
	overflow: 'hidden',
}));

export const ScopeItemTypography = styled(Typography)({
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	whiteSpace: 'nowrap',
});
export const SubstitutesMessage = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-gray'],
}));

export const SubstitutesItemWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: '1fr 11px 2fr',
	gap: theme.spaces.s,
	alignItems: 'center',

	[theme.breakpoints.down(MOBILE_L_BREAKPOINT)]: {
		gridTemplateColumns: '1fr',
	},
}));

export const SubstitutesListWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
}));

export const SubstitutesWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

export const DynamicFieldWrapper = styled('div')({
	flexGrow: 1,
});

export const ArrowForwardWrapper = styled('div')(({ theme }) => ({
	display: 'flex',

	'& svg': {
		width: `${ARROW_ICON_WIDTH}px`,
		height: `${ARROW_ICON_HEIGHT}px`,
	},

	[theme.breakpoints.down(MOBILE_L_BREAKPOINT)]: {
		justifyContent: 'center',
		'& svg': {
			rotate: '90deg',
			width: `${ARROW_ICON_HEIGHT}px`,
			height: `${ARROW_ICON_WIDTH}px`,
		},
	},
}));

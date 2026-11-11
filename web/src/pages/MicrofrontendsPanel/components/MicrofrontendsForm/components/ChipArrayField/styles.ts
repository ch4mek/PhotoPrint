import styled from '@emotion/styled';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const BUTTON_POSITION = 7;

export const ChipArrayFieldWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
	width: '100%',
	flex: 1,

	[`${theme.breakpoints.down(theme.breakpoints.tabletS)}`]: {
		width: '100%',
	},
}));

export const InputWithButtonWrapper = styled('div')(({ theme }) => ({
	position: 'relative',
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
}));

export const AddButton = styled(ActionButton)(({ theme }) => ({
	position: 'absolute',
	right: BUTTON_POSITION,
	top: BUTTON_POSITION,

	background: theme.palette['accent-5'],
	borderRadius: BorderRadiuses.m,
}));

export const ChipsContainer = styled('div')(({ theme }) => ({
	display: 'flex',
	flexWrap: 'wrap',
	gap: theme.spaces.s,
	margin: `${theme.spaces.s}px 0`,
	marginTop: theme.spaces.xs,
}));

import styled from '@emotion/styled';

import { popUpBreakpoints } from '@/constants/popUp';

const HOURS_INPUT_WIDTH = 120;

export const FormWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
}));

export const HoursInputWrapper = styled('div')({
	maxWidth: HOURS_INPUT_WIDTH,
	minWidth: HOURS_INPUT_WIDTH,
});

export const MilitaryDatePicker = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	justifyContent: 'space-between',
}));

export const CountValuesWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.m,

	[theme.breakpoints.down(popUpBreakpoints.tabletXS)]: {
		flexWrap: 'wrap',
		gap: theme.spaces.s,
	},
}));

export const DatePickerWrapper = styled('div')({
	flexGrow: 1,
	width: '100%',
});

export const RequestFieldGroupWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	flexDirection: 'column',
}));

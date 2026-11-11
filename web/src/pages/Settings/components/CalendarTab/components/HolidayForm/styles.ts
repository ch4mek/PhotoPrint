import styled from '@emotion/styled';

export const RadioButtonsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xl6,
}));

export const RadioSectionWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

export const HolidayFieldsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
}));

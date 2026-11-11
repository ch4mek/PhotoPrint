import styled from '@emotion/styled';

export const MonthsList = styled('ul')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
	padding: 0,
	margin: 0,
}));

export const InputAndLabelWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

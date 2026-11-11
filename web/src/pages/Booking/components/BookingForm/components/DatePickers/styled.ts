import styled from '@emotion/styled';

import { FieldHeaderWrapper } from '../FieldHeaderWrapper';

export const DatePickersWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column' as const,
	gap: theme.spaces.m,
}));

export const DatePickersContentWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	gap: theme.spaces.s,
}));

export const DatePickersHeader = styled(FieldHeaderWrapper)(({ theme }) => ({
	color: theme.palette['text-black'],
}));

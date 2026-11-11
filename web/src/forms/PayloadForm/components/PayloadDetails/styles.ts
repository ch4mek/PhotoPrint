import styled from '@emotion/styled';

export const FormContentWrapper = styled('div')(({ theme }) => ({
	width: '100%',
	display: 'flex',
	flexDirection: 'column',
	justifyContent: 'center',
	gap: theme.spaces.l,
}));

export const NeedProjectContainer = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
}));

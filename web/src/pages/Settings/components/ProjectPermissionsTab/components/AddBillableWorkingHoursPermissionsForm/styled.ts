import styled from '@emotion/styled';

const FORM_MAX_WIDTH = 350;

export const FormWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
	maxWidth: FORM_MAX_WIDTH,
	color: theme.palette['text-black'],
	...theme.typography.body['m-400'],
}));

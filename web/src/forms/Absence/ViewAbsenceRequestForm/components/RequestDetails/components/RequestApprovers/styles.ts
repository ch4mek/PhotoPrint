import styled from '@emotion/styled';

export const ApproversList = styled('ul')(({ theme }) => ({
	margin: 0,
	padding: 0,
	listStyle: 'none',

	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

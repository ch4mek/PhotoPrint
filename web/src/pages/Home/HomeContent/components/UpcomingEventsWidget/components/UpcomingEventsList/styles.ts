import styled from '@emotion/styled';

export const UpcomingEventsListWrapper = styled('ul')(({ theme }) => ({
	margin: 0,
	padding: 0,
	listStyle: 'none',

	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,

	width: '100%',
}));

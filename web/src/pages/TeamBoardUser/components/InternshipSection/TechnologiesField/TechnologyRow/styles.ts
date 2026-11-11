import styled from '@emotion/styled';

export const TechnologyRowWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'flex-start',
	gap: theme.spaces.xs,
	width: '100%',
	wordBreak: 'normal',
	overflowWrap: 'break-word',
}));

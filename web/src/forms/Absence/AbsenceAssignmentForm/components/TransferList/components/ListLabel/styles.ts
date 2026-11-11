import styled from '@emotion/styled';

export const ListLabelWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xs,
	alignItems: 'center',
	width: '100%',
}));

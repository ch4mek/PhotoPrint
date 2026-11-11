import styled from '@emotion/styled';

export const FilesGroupWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gap: theme.spaces.s,
	gridTemplateColumns: `repeat(2, 1fr)`,

	[theme.breakpoints.down(theme.breakpoints.tabletS)]: {
		gridTemplateColumns: `repeat(1, 1fr)`,
	},
}));

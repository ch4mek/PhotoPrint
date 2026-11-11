import styled from '@emotion/styled';

export const ProjectAccessTabWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
	height: '100%',
	minHeight: 0,
}));

export const ProjectAccessTabHeader = styled('div')({
	flexShrink: 0,
});

export const ProjectAccessGridWrapper = styled('div')({
	display: 'flex',
	flexDirection: 'column',
	flex: '1 1 0',
	minHeight: 0,
});

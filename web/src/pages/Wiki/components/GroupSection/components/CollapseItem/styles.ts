import styled from '@emotion/styled';

export const CollapseItemIcon = styled('div')<{ expanded: boolean }>(
	({ theme, expanded }) => ({
		display: 'flex',
		justifyContent: 'center',
		alignItems: 'start',

		color: theme.palette['graphics-light-gray'],

		...(expanded && {
			transform: 'rotate(90deg)',
			transition: 'all 0.2s ease-in',
		}),
	})
);

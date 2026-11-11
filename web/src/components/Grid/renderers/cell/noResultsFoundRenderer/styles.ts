import styled from '@emotion/styled';
import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';

export const NoResultFoundContainer = styled('div')<{ rowHeight?: number }>(
	({ theme, rowHeight }) => ({
		gridColumn: '1/100',
		width: '100%',
		display: 'flex',
		alignItems: 'center',
		height: rowHeight || DEFAULT_GRID_ROW_HEIGHT,
		borderBottom: `1px solid ${theme.palette['graphics-divider-2']}`,
		color: theme.palette['text-black'],
		padding: `0 ${theme.spaces.s}px`,
	})
);

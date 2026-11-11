import styled from '@emotion/styled';

export const MonthCellWrapper = styled('div')<{ cellWidth: number }>(
	({ cellWidth }) => ({
		height: '100%',
		width: cellWidth,
		position: 'relative',
		overflow: 'visible',
	})
);

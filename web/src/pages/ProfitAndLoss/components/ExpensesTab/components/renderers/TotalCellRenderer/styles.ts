import styled from '@emotion/styled';

const TOTAL_CELL_INNER_SPACING = 2;

export const TotalCellWrapper = styled('div')({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'end',
	width: '100%',
	height: '100%',
	paddingRight: TOTAL_CELL_INNER_SPACING,
});

import styled from '@emotion/styled';

import {
	DEFAULT_DISABLED_GRID_CELL_OPACITY,
	DEFAULT_OPACITY,
} from '@/constants/opacities';

export const CellWrapper = styled('div')<{ isMarkAsPast: boolean }>(
	({ isMarkAsPast }) => ({
		height: '100%',
		width: ' 100%',
		textOverflow: 'ellipsis',
		overflow: 'hidden',
		opacity: isMarkAsPast
			? DEFAULT_DISABLED_GRID_CELL_OPACITY
			: DEFAULT_OPACITY,
	})
);

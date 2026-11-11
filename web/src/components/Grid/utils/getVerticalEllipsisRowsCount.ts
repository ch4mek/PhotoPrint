import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';

export const getVerticalEllipsisRowsCount = (
	rowHeight = DEFAULT_GRID_ROW_HEIGHT,
	lineHeight = DEFAULT_GRID_ROW_HEIGHT
) => Math.floor(rowHeight / lineHeight);

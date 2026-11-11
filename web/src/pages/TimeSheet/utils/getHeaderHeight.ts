import { DEFAULT_GRID_HEADER_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';

export const getHeaderRowHeight = (isEngineer: boolean): number => {
	if (!isEngineer) {
		return 0;
	}

	return DEFAULT_GRID_HEADER_ROW_HEIGHT;
};

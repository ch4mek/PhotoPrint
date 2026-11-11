import { MIN_FULLNAME_COLUMN_WIDTH } from '@/constants/dashboard';
import { GridColumnWidthsConfiguration } from '@/types/grid';

export const contractexpirationGridColumnWidths: GridColumnWidthsConfiguration =
	{
		user: {
			width: MIN_FULLNAME_COLUMN_WIDTH,
		},
		dates: {
			width: 100,
			fixed: true,
		},
	};

export const CONTRACT_EXPIRATION_GRID_COLUMNS = {
	userKey: 'user',
	datesKey: 'dates',
};

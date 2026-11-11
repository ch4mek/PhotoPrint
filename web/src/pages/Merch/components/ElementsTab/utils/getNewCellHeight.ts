import { EXPANDED_ROWS_HEIGHT, FIRST_ROWS_HEIGHT } from '../constants';

import { ONE_SIZE_ID } from '@/pages/Merch/constants';
import { MerchSizeType } from '@/types/configuration';

export const getNewCellHeight = (
	merchStorages: string[],
	merchSizes: MerchSizeType[],
	isClothing = true
) =>
	merchStorages.length *
		EXPANDED_ROWS_HEIGHT *
		merchSizes.filter(({ id }) =>
			isClothing ? id !== ONE_SIZE_ID : id === ONE_SIZE_ID
		).length +
	FIRST_ROWS_HEIGHT;

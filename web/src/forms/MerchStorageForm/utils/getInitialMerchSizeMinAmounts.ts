import {
	INITIAL_CLOTHING_COUNT,
	INITIAL_NON_CLOTHING_COUNT,
} from '../constants';
import { AmountItem } from '../types';

import { ONE_SIZE_ID } from '@/pages/Merch/constants';
import { MerchSizeType } from '@/types/configuration';

export const getInitialMerchSizeAmounts = (
	merchSizes: MerchSizeType[],
	isClothing = false
) =>
	(merchSizes
		.map(({ id: sizeId }) => ({
			sizeId,
			count: isClothing ? INITIAL_CLOTHING_COUNT : INITIAL_NON_CLOTHING_COUNT,
		}))
		.filter(({ sizeId }) =>
			isClothing ? sizeId !== ONE_SIZE_ID : sizeId === ONE_SIZE_ID
		) as AmountItem[]) || [];

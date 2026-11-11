import { MinAmountItem } from '../types';

import { MerchSizeType } from '@/types/configuration';

export const getInitialMerchSizeMinAmounts = (
	merchStorages: MerchSizeType[],
	merchSizes: MerchSizeType[]
) =>
	(merchStorages.flatMap(({ id: storageId }) =>
		merchSizes.map(({ id: sizeId }) => ({
			storageId: +storageId,
			sizeId,
			minAmount: 0,
		}))
	) as MinAmountItem[]) || [];

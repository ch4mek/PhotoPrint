import { MerchKitItemSize } from '@/forms/MerchKitsTransactionForm/types';
import { CompoundItem } from '@/pages/Merch/components/KitsTab/types';

export const getInitialMerchSizes = (
	itemSizes: MerchKitItemSize[],
	clothingItem: CompoundItem
) =>
	itemSizes
		.find((item) => item.itemId === clothingItem.itemId)
		?.sizeCounts.map((size) => ({
			...size,
			count: 0,
		})) || [];

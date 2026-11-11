import {
	CompoundItem,
	MerchKitCompoundPayload,
} from '@/pages/Merch/components/KitsTab/types';
import { MerchElementsLookupResponse } from '@/pages/Merch/components/StorageTab/types';

export const getCompoundAutoSelectValueName = (
	compounds: MerchKitCompoundPayload[],
	index: number,
	allMerchElements?: MerchElementsLookupResponse[],
	kitCompounds?: CompoundItem[]
) => {
	const itemId = compounds[index].itemId;

	if (!itemId) {
		return;
	}

	return (
		allMerchElements?.find(({ id }) => itemId === id)?.name ??
		kitCompounds?.find(({ itemId: kitItemId }) => kitItemId === itemId)?.name
	);
};

import {
	MerchKitItemSize,
	SizeCount,
} from '@/forms/MerchKitsTransactionForm/types';
import { CompoundItem } from '@/pages/Merch/components/KitsTab/types';
import { MerchSizeType } from '@/types/configuration';

function sortSizes(sizes: SizeCount[], order: MerchSizeType[]) {
	const orderMap = new Map(
		order.map((size, index) => [size.id.toLowerCase(), index])
	);

	return sizes.slice().sort((a, b) => {
		const indexA = orderMap.get(a.sizeId.toLowerCase()) ?? Infinity;
		const indexB = orderMap.get(b.sizeId.toLowerCase()) ?? Infinity;

		return indexA - indexB;
	});
}

export function mergeCompoundsWithSizes(
	compounds: CompoundItem[],
	itemSizes: MerchKitItemSize[],
	sizeOrder: MerchSizeType[]
) {
	return compounds
		.filter((compound) => compound.isClothing)
		.map((compound) => {
			const sizes =
				itemSizes.find((item) => item.itemId === compound.itemId)?.sizeCounts ??
				[];

			return {
				...compound,
				sizes: sortSizes(sizes, sizeOrder),
			};
		});
}

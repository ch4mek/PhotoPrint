import { ONE_SIZE_ID } from '@/pages/Merch/constants';
import { MerchSizeType } from '@/types/configuration';

export const filterSizeAndJoinWithComma = (merchSizes: MerchSizeType[]) =>
	merchSizes
		.filter(({ id: sizeId }) => sizeId !== ONE_SIZE_ID)
		.map(({ id: sizeId }) => sizeId)
		.join(', ');

import { MerchElementsFormValues } from '@/forms/MerchElementForm/types';
import { ONE_SIZE_ID } from '@/pages/Merch/constants';

export const filterMerchValuesDueToIsClothing = (
	values: MerchElementsFormValues
) =>
	values.sizeMinAmounts.filter(({ sizeId }) =>
		values.isClothing ? sizeId !== ONE_SIZE_ID : sizeId === ONE_SIZE_ID
	);

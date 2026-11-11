import { MerchElementsFormValues } from '@/forms/MerchElementForm/types';

export const findValueByIndex = (
	values: MerchElementsFormValues,
	sizeId: string,
	storageId: number
) =>
	values.sizeMinAmounts.findIndex(
		({ sizeId: valuesSizeId, storageId: valuesStorageId }) =>
			sizeId === valuesSizeId && storageId === valuesStorageId
	);

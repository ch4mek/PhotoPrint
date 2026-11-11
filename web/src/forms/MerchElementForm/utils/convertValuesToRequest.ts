import { MerchElementsFormValues } from '../types';

import { filterMerchValuesDueToIsClothing } from '@/pages/Merch/components/ElementsTab/utils/filterMerchValuesDueToIsClothing';

export const convertValuesToRequest = (
	values: MerchElementsFormValues,
	imageFileId: string | null,
	id?: number | string
) => ({
	id,
	name: values.name,
	photoFileId: imageFileId,
	supplier: values.supplier || null,
	description: values.description || null,
	isClothing: values.isClothing,
	sizeMinAmounts: filterMerchValuesDueToIsClothing(values),
});

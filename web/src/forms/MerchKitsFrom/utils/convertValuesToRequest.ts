import { MerchKitsFormValues } from '../types';

import { UpdateMerchKitPayload } from '@/queries/merchKits/types';

export const convertValuesToRequest = (
	values: MerchKitsFormValues,
	imageFileId: string | null,
	id?: number | string
): UpdateMerchKitPayload => ({
	id,
	name: values.name,
	photoFileId: imageFileId,
	description: values.description || null,
	minAmounts: values.minAmounts,
	compounds: values.compounds,
});

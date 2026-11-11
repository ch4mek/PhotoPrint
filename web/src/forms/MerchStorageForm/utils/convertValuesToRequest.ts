import { MerchStorageFormValues } from '../types';

export const convertValuesToRequest = (values: MerchStorageFormValues) => ({
	itemId: values.itemId,
	warehouseId: values.warehouseId,
	transactionType: values.transactionType!.id,
	notes: values.notes || null,
	userRecipientId: values.userRecipientId ?? null,
	warehouseRecipientId: values.warehouseRecipientId ?? null,
	sizeCounts: values.sizeCounts,
});

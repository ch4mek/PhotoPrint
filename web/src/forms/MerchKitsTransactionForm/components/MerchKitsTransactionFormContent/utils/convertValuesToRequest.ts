import { MerchKitsTransactionFormValues } from '@/forms/MerchKitsTransactionForm/types';

export const convertValuesToRequest = (
	values: MerchKitsTransactionFormValues
) => ({
	warehouseId: values.warehouseId,
	transactionType: values.transactionType,
	amount: values.amount,
	kitId: Number(values.kitId),
	notes: values.notes || null,
	userRecipientId: values.userRecipientId ?? null,
	warehouseRecipientId: values.warehouseRecipientId ?? null,
	sizeCounts: values.sizeCounts,
});

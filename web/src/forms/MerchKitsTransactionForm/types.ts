import { AutoselectOption } from '@/types/common';

export interface MerchKitsTransactionFormProps {
	handleClose: VoidFunction;
	handleSubmit: (
		values: MerchKitsTransactionFormValues
	) => Promise<MerchKitsTransactionFormValues>;
	kitId: number | string;
}

export interface MerchKitsTransactionFormValues {
	amount: number;
	transactionType: string;
	kitId: number | string;
	sizeCounts: SizeCount[];
	warehouseId: number;
	userRecipientId?: number | null;
	warehouseRecipientId?: number | null;
	notes?: string | null;
	recipient?: AutoselectOption | null;
}

export interface SizeCount {
	sizeId: string;
	count: number;
}

export interface MerchKitItemSize {
	itemId: number;
	countInKit: number;
	sizeCounts: SizeCount[];
}

export interface MerchKitStorageCounts {
	availableAmountInStorage: number;
	itemSizes: MerchKitItemSize[];
}

export interface TransactionTypesObject {
	[index: string]: {
		id: string;
		value: string;
	};
}

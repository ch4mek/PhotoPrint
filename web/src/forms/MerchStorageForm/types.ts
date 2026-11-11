import { AutoselectOption } from '@/types/common';
import { MerchTransactionType } from '@/types/configuration';

export type MerchTransactionOption = Pick<MerchTransactionType, 'id' | 'name'>;

interface MerchStorageValues {
	itemId: number;
	sizeCounts: AmountItem[];
	warehouseId: number;
	userRecipientId?: number | null;
	warehouseRecipientId?: number | null;
	notes?: string | null;
	recipient?: AutoselectOption | null;
}

export type MerchStorageFormValues = MerchStorageValues & {
	transactionType: MerchTransactionOption | null;
};

export type MerchStorageSubmitFormValues = MerchStorageValues & {
	transactionType: string;
};

export interface AmountItem {
	sizeId: string;
	count: number;
}

export interface MerchStorageFormProps {
	handleClose: VoidFunction;
	handleSubmit: (
		values: MerchStorageSubmitFormValues
	) => Promise<MerchStorageFormValues>;
	isAfterElementAdding?: boolean;
	merchId: number | string;
}

export interface TransactionTypesObject {
	[index: string]: MerchTransactionOption;
}

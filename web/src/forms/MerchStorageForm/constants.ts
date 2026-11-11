import { TransactionTypesObject } from './types';

export const TRANSACTION_FIELD_TITLE = 'Transaction';
export const FORM_NAME = 'Transaction adding form';

export const transactionTypes: TransactionTypesObject = {
	WriteOff: {
		id: 'WriteOff',
		name: 'Write-Off',
	},
	Receipt: {
		id: 'Receipt',
		name: 'Receipt',
	},
	Transfer: {
		id: 'Transfer',
		name: 'Transfer',
	},
};

export const transactionTypeOptions = Object.values(transactionTypes);

export const merchStorageFieldData = {
	storage: {
		name: 'warehouseId',
		label: 'Storage',
	},
	transactionType: {
		name: 'transactionType',
		label: 'Transaction type',
	},
	recipient: {
		name: 'recipient',
		label: 'Recipient',
	},
	notes: {
		name: 'notes',
		label: 'Notes',
	},
};

export const INITIAL_CLOTHING_COUNT = 0;
export const INITIAL_NON_CLOTHING_COUNT = 1;

export const INITIAL_MERCH_STORAGE_ID = 1;

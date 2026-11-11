import { TransactionTypesObject } from './types';

export const TRANSACTION_FIELD_TITLE = 'Transaction';

export const FORM_NAME = 'Transaction form';

export const MIN_AMOUNT = 0;
export const MAX_AMOUNT = 9999;

export const TransactionTypes: TransactionTypesObject = {
	WriteOff: {
		id: 'WriteOff',
		value: 'Write-Off',
	},
	Receipt: {
		id: 'Receipt',
		value: 'Receipt',
	},
	Transfer: {
		id: 'Transfer',
		value: 'Transfer',
	},
};

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
	kitAmount: {
		name: 'kitAmount',
	},
	notes: {
		name: 'notes',
		label: 'Notes',
	},
};

export const INITIAL_MERCH_STORAGE_ID = 1;
export const INITIAL_MERCH_KIT_AMOUNT = 1;

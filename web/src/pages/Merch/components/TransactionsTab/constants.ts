import {
	fromToCurrencyObject,
	fromToDateObject,
} from '@/constants/defaultValues';

export const DEFAULT_MERCH_TRANSACTIONS_GRID_FILTERS = {
	idFilter: '',
	dateFilter: fromToDateObject,
	nameFilter: '',
	transactionTypeFilter: [],
	amountFilter: fromToCurrencyObject,
	senderFilter: [],
	storageFilter: [],
	creatorFilter: '',
	notesFilter: '',
	recipientFilter: [],
};

export const TransactionTypes = [
	{
		id: 'WriteOff',
		name: 'Write-off',
	},
	{
		id: 'Transfer',
		name: 'Transfer',
	},
	{
		id: 'Receipt',
		name: 'Receipt',
	},
];

export const TRANSACTIONS_TAB_GRID_COLUMNS = {
	id: {
		key: 'orderTransactionId',
		name: 'Id',
		sortable: true,
	},
	date: {
		key: 'date',
		name: 'Date',
		sortable: true,
	},
	transactionType: {
		key: 'type',
		name: 'Transact. type',
		sortable: true,
	},
	name: {
		key: 'elementName',
		name: 'Name',
		sortable: true,
	},
	size: {
		key: 'size',
		name: 'Size',
		sortable: false,
	},
	amount: {
		key: 'sumAmount',
		name: 'Amount',
		sortable: true,
	},
	storage: {
		key: 'storage',
		name: 'Storage',
		sortable: false,
	},
	sender: {
		key: 'senderName',
		name: 'Sender',
		sortable: true,
	},
	creator: {
		key: 'creator',
		name: 'Creator',
		sortable: true,
	},
	recipient: {
		key: 'recipient',
		name: 'Recipient',
		sortable: true,
	},
	notes: {
		key: 'notes',
		name: 'Notes',
		sortable: false,
	},
};

export const TRANSACTIONS_TAB_COLUMNS_WIDTH = {
	id: {
		width: 88,
		fixed: true,
	},
	date: {
		width: 100,
		fixed: true,
	},
	transactionType: {
		width: 130,
		fixed: true,
	},
	name: {
		width: 250,
		fixed: false,
	},
	size: {
		width: 195,
		fixed: true,
	},
	amount: {
		width: 96,
		fixed: true,
	},
	storage: {
		width: 100,
		fixed: true,
	},
	sender: {
		width: 91,
		fixed: true,
	},
	creator: {
		width: 200,
		fixed: true,
	},
	recipient: {
		width: 200,
		fixed: true,
	},
	notes: {
		width: 260,
		fixed: false,
	},
};

export const TRANSACTION_TYPE_ORDER_BY = 'orderWeight';

import { fromToCurrencyObject } from '@/constants/defaultValues';
import { MERCH_STATUS_DEFAULT_FILTER } from '@/pages/Merch/constants';

export const LOWERCASED_RUNNING_OUT_STATUS = 'runningout';

export const MIN_AMOUNT_CLOTHING_IN_STORAGE_TRANSACTION = 0;
export const MIN_AMOUNT_IN_STORAGE_TRANSACTION = 1;
export const MAX_AMOUNT_IN_TRANSACTION = 9999;

export const DEFAULT_MERCH_STORAGE_GRID_FILTERS = {
	idFilter: '',
	nameFilter: '',
	storageFilter: [],
	statusFilter: MERCH_STATUS_DEFAULT_FILTER,
	supplierFilter: '',
	descriptionFilter: '',
	totalAmountFilter: fromToCurrencyObject,
};

export const STORAGE_TAB_GRID_COLUMNS = {
	id: {
		key: 'id',
		name: 'Id',
		sortable: true,
	},
	name: {
		key: 'name',
		name: 'Name',
		sortable: true,
	},
	storage: {
		key: 'storage',
		name: 'Storage',
		sortable: false,
	},
	size: {
		key: 'size',
		name: 'Size',
		sortable: false,
	},
	totalAmount: {
		key: 'totalAmount',
		name: 'Amount',
		sortable: true,
	},
	status: {
		key: 'status',
		name: 'Status',
		sortable: true,
	},
	supplier: {
		key: 'supplier',
		name: 'Supplier',
		sortable: true,
	},
	description: {
		key: 'description',
		name: 'Description',
		sortable: false,
	},
};

export const STORAGE_TAB_COLUMNS_WIDTH = {
	id: {
		width: 67,
		fixed: true,
	},
	name: {
		width: 250,
		maxWidth: 350,
		fixed: false,
	},
	storage: {
		width: 168,
		fixed: true,
	},
	size: {
		width: 195,
		fixed: true,
	},
	totalAmount: {
		width: 96,
		fixed: true,
	},
	status: {
		width: 88,
		maxWidth: 150,
		fixed: false,
	},
	supplier: {
		width: 200,
		fixed: true,
	},
	description: {
		width: 260,
		fixed: false,
	},
};

export const INITIAL_ORDER_BY_COLUMN = STORAGE_TAB_GRID_COLUMNS.status.key;

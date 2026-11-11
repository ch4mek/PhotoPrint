import { fromToCurrencyObject } from '@/constants/defaultValues';
import { MERCH_STATUS_DEFAULT_FILTER } from '@/pages/Merch/constants';

export const MERCH_KITS_FIELD_TITLE = 'Kit';

export const DEFAULT_MERCH_KITS_GRID_FILTERS = {
	idFilter: '',
	nameFilter: '',
	storageFilter: [],
	statusFilter: MERCH_STATUS_DEFAULT_FILTER,
	compoundFilter: '',
	descriptionFilter: '',
	possibleKitsFilter: fromToCurrencyObject,
};

export const KITS_TAB_GRID_COLUMNS = {
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
	compound: {
		key: 'compound',
		name: 'Compound',
		sortable: false,
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
	possibleKits: {
		key: 'possibleKits',
		name: 'Possible kits',
		sortable: true,
	},
	status: {
		key: 'status',
		name: 'Status',
		sortable: true,
	},
	description: {
		key: 'description',
		name: 'Description',
		sortable: false,
	},
};

export const KITS_TAB_COLUMNS_WIDTH = {
	id: {
		width: 67,
		fixed: true,
	},
	name: {
		width: 250,
		maxWidth: 350,
		fixed: false,
	},
	compound: {
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
	possibleKits: {
		width: 120,
		fixed: true,
	},
	status: {
		width: 88,
		maxWidth: 120,
		fixed: false,
	},
	description: {
		width: 260,
		fixed: false,
	},
};

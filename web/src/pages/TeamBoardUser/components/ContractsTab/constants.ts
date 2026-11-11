import { fromToDateObject } from '@/constants/defaultValues';

export const BUTTON_ADD_TITLE = 'Add contract';

export const CONTRACTS_GRID_WIDTH = {
	contractFrom: {
		fixed: true,
		width: 107,
	},
	contractTo: {
		fixed: true,
		width: 101,
	},
	fireDate: {
		fixed: true,
		width: 101,
	},
	fireType: 200,
	fireReason: 200,
	fireDescription: 200,
};

export const CONTRACTS_GRID_COLUMNS = {
	contractFrom: {
		key: 'contractFrom',
		name: 'Start date',
		width: CONTRACTS_GRID_WIDTH.contractFrom.width,
	},
	contractTo: {
		key: 'contractTo',
		name: 'End date',
		width: CONTRACTS_GRID_WIDTH.contractTo.width,
	},
	fireDate: {
		key: 'fireDate',
		name: 'Fire date',
		width: CONTRACTS_GRID_WIDTH.fireDate.width,
	},
	fireType: {
		key: 'fireType',
		name: 'Fire type',
		width: CONTRACTS_GRID_WIDTH.fireType,
	},
	fireReason: {
		key: 'fireReason',
		name: 'Fire reason',
		width: CONTRACTS_GRID_WIDTH.fireReason,
	},
	fireDescription: {
		key: 'fireDescription',
		name: 'Fire description',
		width: CONTRACTS_GRID_WIDTH.fireDescription,
	},
};

export const CONTRACTS_GRID_COLUMNS_AMOUNT = Object.keys(
	CONTRACTS_GRID_COLUMNS
).length;

export const CONTRACTS_FILTERS_INITIAL_STATE = {
	contractFrom: fromToDateObject,
	contractTo: fromToDateObject,
	fireDate: fromToDateObject,
	fireType: [],
	fireReason: '',
	fireDescription: '',
};

export const MOCK_ROW = {
	id: 0,
	contractFrom: null,
	contractTo: null,
	showSkeleton: true,
};

export const CONTRACT_CREATE_SUCCESS_TOAST_MESSAGE = 'Contracts are updated';

export const CONTRACT_TIME_PERIOD_TOAST =
	'Contract for this period already exists. Please check and try again';

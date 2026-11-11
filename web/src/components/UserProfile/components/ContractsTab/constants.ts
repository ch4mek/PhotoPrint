import { fromToDateObject } from '@/constants/defaultValues';

export const contractsGridWidth = {
	contractFrom: 300,
	contractTo: 300,
};

export const contractsGridColumns = {
	contractFrom: {
		key: 'contractFrom',
		width: contractsGridWidth.contractFrom,
	},
	contractTo: {
		key: 'contractTo',
		width: contractsGridWidth.contractTo,
	},
};

export const CONTRACTS_GRID_COLUMNS_AMOUNT =
	Object.keys(contractsGridColumns).length;

export const contractsFiltersInitialState = {
	contractFrom: fromToDateObject,
	contractTo: fromToDateObject,
	fireDate: fromToDateObject,
	fireType: [],
	fireReason: '',
	fireDescription: '',
};

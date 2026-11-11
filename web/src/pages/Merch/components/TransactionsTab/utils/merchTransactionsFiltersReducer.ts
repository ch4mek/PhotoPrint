import { DEFAULT_MERCH_TRANSACTIONS_GRID_FILTERS } from '../constants';
import {
	MerchTransactionsFiltersAction,
	MerchTransactionsFiltersActionKeys,
	TransactionsTabColumnsPropsFilters,
} from '../types';

import { FromToProps } from '@/components/Grid';

export const merchTransactionsFiltersReducer = (
	state: TransactionsTabColumnsPropsFilters,
	{ type, payload }: MerchTransactionsFiltersAction
): TransactionsTabColumnsPropsFilters => {
	switch (type) {
		case MerchTransactionsFiltersActionKeys.set:
			return payload as TransactionsTabColumnsPropsFilters;
		case MerchTransactionsFiltersActionKeys.id:
			return { ...state, idFilter: payload as string };
		case MerchTransactionsFiltersActionKeys.name:
			return { ...state, nameFilter: payload as string };
		case MerchTransactionsFiltersActionKeys.amount:
			return { ...state, amountFilter: payload as FromToProps<string> };
		case MerchTransactionsFiltersActionKeys.creator:
			return { ...state, creatorFilter: payload as string };
		case MerchTransactionsFiltersActionKeys.date:
			return { ...state, dateFilter: payload as FromToProps<Date> };
		case MerchTransactionsFiltersActionKeys.notes:
			return { ...state, notesFilter: payload as string };
		case MerchTransactionsFiltersActionKeys.recipient:
			return { ...state, recipientFilter: payload as string[] };
		case MerchTransactionsFiltersActionKeys.sender:
			return { ...state, senderFilter: payload as string[] };
		case MerchTransactionsFiltersActionKeys.storage:
			return { ...state, storageFilter: payload as string[] };
		case MerchTransactionsFiltersActionKeys.transactionType:
			return { ...state, transactionTypeFilter: payload as string[] };
		case MerchTransactionsFiltersActionKeys.clear:
			return { ...DEFAULT_MERCH_TRANSACTIONS_GRID_FILTERS };
		default:
			return state;
	}
};

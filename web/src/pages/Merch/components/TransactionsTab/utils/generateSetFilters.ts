import { Dispatch } from 'react';

import {
	MerchTransactionsFiltersAction,
	MerchTransactionsFiltersActionKeys,
	MerchTransactionsGridFilterSetters,
	TransactionsTabColumnsPropsFilters,
} from '../types';

import { FromToProps } from '@/components/Grid';

export const generateSetFilters = (
	dispatch: Dispatch<MerchTransactionsFiltersAction>
): MerchTransactionsGridFilterSetters => ({
	id: (id: string) =>
		dispatch({
			type: MerchTransactionsFiltersActionKeys.id,
			payload: id,
		}),
	name: (name: string) =>
		dispatch({
			type: MerchTransactionsFiltersActionKeys.name,
			payload: name,
		}),
	amount: (amount: FromToProps<string>) =>
		dispatch({
			type: MerchTransactionsFiltersActionKeys.amount,
			payload: amount,
		}),
	creator: (creator: string) =>
		dispatch({
			type: MerchTransactionsFiltersActionKeys.creator,
			payload: creator,
		}),
	date: (date: FromToProps<Date>) =>
		dispatch({
			type: MerchTransactionsFiltersActionKeys.date,
			payload: date,
		}),
	notes: (notes: string) =>
		dispatch({
			type: MerchTransactionsFiltersActionKeys.notes,
			payload: notes,
		}),
	recipient: (recipient: string[]) =>
		dispatch({
			type: MerchTransactionsFiltersActionKeys.recipient,
			payload: recipient,
		}),
	transactionType: (transactionType: string[]) =>
		dispatch({
			type: MerchTransactionsFiltersActionKeys.transactionType,
			payload: transactionType,
		}),
	sender: (sender: string[]) =>
		dispatch({
			type: MerchTransactionsFiltersActionKeys.sender,
			payload: sender,
		}),
	storage: (storage: string[]) =>
		dispatch({
			type: MerchTransactionsFiltersActionKeys.storage,
			payload: storage,
		}),
	set: (newFilters: TransactionsTabColumnsPropsFilters) =>
		dispatch({
			type: MerchTransactionsFiltersActionKeys.set,
			payload: newFilters,
		}),
	clear: () => dispatch({ type: MerchTransactionsFiltersActionKeys.clear }),
});

import { FromToProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { OdataParams } from '@/types/common';

export interface TransactionsTabColumnsPropsFilters {
	idFilter: string;
	dateFilter: FromToProps<Date>;
	nameFilter: string;
	transactionTypeFilter: string[];
	amountFilter: FromToProps<string>;
	senderFilter: string[];
	creatorFilter: string;
	notesFilter: string;
	recipientFilter: string[];
	storageFilter: string[];
}

export interface TransactionsTabColumnsProps {
	filters: TransactionsTabColumnsPropsFilters;
	setFilters: MerchTransactionsGridFilterSetters;
	allMerchTransactionsNames: string[];
	allMerchTransactionsCreators: string[];
	allMerchSenders: Array<SelectionModalOption>;
	allMerchStorages: Array<SelectionModalOption>;
	allMerchTransactionTypes: Array<SelectionModalOption>;
	allRecipients: Array<SelectionModalOption>;
}

export interface BodyFilters {
	idFrom: string | null;
	idTo: string | null;
}

export interface GetMerchTransactionsQueryParams
	extends OdataParams<TransactionsTabColumnsPropsFilters> {}

export interface TransactionItemResponse {
	amount: number;
	id: number;
	size: string;
	wearhouseName: string;
}

export interface MerchTransaction {
	id: number;
	elementName: string;
	elementFileId: string;
	elementDescription: string;
	creator: string;
	date: string;
	isClothing: boolean;
	notes: string;
	recipient: string;
	senderName: string;
	sumAmount: number;
	transactionIdFrom: number;
	transactionIdTo: number;
	totalAmount: number;
	type: string;
	warehouseNames: string[];
	orderTransactionId: number;
	transactions: TransactionItemResponse[];
}
export interface MerchTransactionsGridRow {
	id: number;
	elementName: string;
	elementDescription: string;
	transactionIdFrom: number;
	elementFileId: string;
	transactionIdTo: number;
	date: string;
	type: string;
	isClothing: boolean;
	transactions: TransactionItemResponse[];
	itemsRowHeight: number;
	sumAmount: number;
	orderTransactionId: string;
	senderName: string;
	storage: string;
	expanded?: boolean;
	creator: string;
	recipient: string;
	notes: string;
	color: string;
}

export enum MerchTransactionsFiltersActionKeys {
	id = 'id',
	date = 'date',
	name = 'name',
	transactionType = 'transactionType',
	amount = 'amount',
	sender = 'sender',
	storage = 'storage',
	creator = 'creator',
	recipient = 'recipient',
	notes = 'notest',
	set = 'set',
	clear = 'clear',
}

export type MerchTransactionsGridFilterSetters = {
	id: (id: string) => void;
	date: (date: FromToProps<Date>) => void;
	name: (name: string) => void;
	transactionType: (transactionType: string[]) => void;
	amount: (amount: FromToProps<string>) => void;
	sender: (sender: string[]) => void;
	storage: (storage: string[]) => void;
	creator: (creator: string) => void;
	notes: (notes: string) => void;
	recipient: (recipient: string[]) => void;
	set: (filters: TransactionsTabColumnsPropsFilters) => void;
	clear: VoidFunction;
};

export type MerchTransactionsFiltersAction = {
	type: MerchTransactionsFiltersActionKeys;
	payload?:
		| string
		| string[]
		| TransactionsTabColumnsPropsFilters
		| FromToProps<string>
		| FromToProps<Date>;
};

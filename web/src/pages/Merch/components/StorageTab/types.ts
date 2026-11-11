import { FromToProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { OdataParams } from '@/types/common';

export interface StorageTabColumnsPropsFilters {
	idFilter: string;
	nameFilter: string;
	storageFilter: string[];
	statusFilter: string[];
	totalAmountFilter: FromToProps<string>;
	supplierFilter: string;
	descriptionFilter: string;
}

export interface StorageTabColumnsProps {
	filters: StorageTabColumnsPropsFilters;
	setFilters: MerchStorageGridFilterSetters;
	allMerchStorages: Array<SelectionModalOption>;
	allMerchStatuses: Array<SelectionModalOption>;
	allMerchStorageNames: string[];
}

export interface BodyFilters {
	filters: { filterColumn: string | null; filterValue: string | null }[];
	sortOrder: string | null;
	sortColumn: string | null;
	storageIds: string[] | null;
	statusIds: string[] | null;
	amountMin: number | null;
	amountMax: number | null;
}

export interface GetMerchStorageQueryParams
	extends OdataParams<StorageTabColumnsPropsFilters> {}

export interface MinAmountItemResponse {
	amount: number;
	storageName: string;
	sizeId: string;
	status: string;
}

export interface StorageAvailabilityStatusItem {
	storageName: string;
	status?: string;
}

export interface MerchStorage {
	id: number;
	name: string;
	photoFileId: string;
	supplier: string;
	description: string;
	totalAmount: number;
	status: string;
	allStorages: string[];
	isClothing: boolean;
	sizeAmounts: MinAmountItemResponse[];
	storageAvailabilityStatuses: StorageAvailabilityStatusItem[];
}

export interface MerchStorageGridRow {
	id: number;
	name: string;
	status: string;
	supplier: string;
	description: string;
	storage: string[];
	size: string[];
	photoFileId: string;
	isClothing: boolean;
	itemsRowHeight: number;
	totalAmount: number;
	sizeAmounts: MinAmountItemResponse[];
	storageAvailabilityStatuses: StorageAvailabilityStatusItem[];
	handleEditClick: (itemId?: string | number) => () => void;
	expanded?: boolean;
	color: string;
}

export enum MerchStorageFiltersActionKeys {
	id = 'id',
	name = 'name',
	status = 'status',
	storage = 'storage',
	supplier = 'supplier',
	description = 'description',
	totalAmount = 'totalAmount',
	set = 'set',
	clear = 'clear',
}

export type MerchStorageGridFilterSetters = {
	id: (id: string) => void;
	name: (name: string) => void;
	status: (status: string[]) => void;
	storage: (storage: string[]) => void;
	supplier: (supplier: string) => void;
	description: (description: string) => void;
	totalAmount: (totalAmount: FromToProps<string>) => void;
	set: (filters: StorageTabColumnsPropsFilters) => void;
	clear: VoidFunction;
};

export type MerchStorageFiltersAction = {
	type: MerchStorageFiltersActionKeys;
	payload?:
		| string
		| string[]
		| StorageTabColumnsPropsFilters
		| FromToProps<string>;
};

export interface MerchElementsLookupResponse {
	id: number;
	name: string;
	isClothing: boolean;
}

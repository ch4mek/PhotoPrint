import { StorageAvailabilityStatusItem } from '../StorageTab/types';

import { FromToProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { OdataParams } from '@/types/common';

export interface KitsTabColumnsPropsFilters {
	idFilter: string;
	nameFilter: string;
	compoundFilter: string;
	storageFilter: string[];
	possibleKitsFilter: FromToProps<string>;
	statusFilter: string[];
	descriptionFilter: string;
}

export enum MerchKitsFiltersActionKeys {
	id = 'id',
	name = 'name',
	status = 'status',
	storage = 'storage',
	compound = 'compound',
	description = 'description',
	possibleKits = 'possibleKits',
	set = 'set',
	clear = 'clear',
}

export type MerchKitsFiltersAction = {
	type: MerchKitsFiltersActionKeys;
	payload?:
		| string
		| string[]
		| KitsTabColumnsPropsFilters
		| FromToProps<string>;
};

export interface BodyFilters {
	filters: { filterColumn: string | null; filterValue: string | null }[];
	sortOrder: string | null;
	sortColumn: string | null;
	storageIds: string[] | null;
	statusIds: string[] | null;
	amountMin: number | null;
	amountMax: number | null;
}

export interface GetMerchKitsQueryParams
	extends OdataParams<KitsTabColumnsPropsFilters> {}

export interface MerchKitsGridFilterSetters {
	id: (id: string) => void;
	name: (name: string) => void;
	status: (status: string[]) => void;
	storage: (storage: string[]) => void;
	compound: (supplier: string) => void;
	description: (description: string) => void;
	possibleKits: (totalAmount: FromToProps<string>) => void;
	set: (filters: KitsTabColumnsPropsFilters) => void;
	clear: VoidFunction;
}

export interface KitsTabColumnsProps {
	filters: KitsTabColumnsPropsFilters;
	setFilters: MerchKitsGridFilterSetters;
	allMerchStorages: Array<SelectionModalOption>;
	allMerchStatuses: Array<SelectionModalOption>;
	allMerchStorageNames: string[];
}

export interface PossibleKitsItemResponse {
	possibleKitAmount: number;
	storageName: string;
	sizeId: string;
	status: string;
}

export interface CompoundItem {
	itemId: number;
	isClothing: boolean;
	name: string;
	amountInKit: number;
}

export interface MerchKitCompoundPayload {
	itemId?: number | null;
	amountInKit: number;
}

export interface MerchKitMinAmount {
	storageId: number;
	minimumAmount: number;
}

export interface MerchKits {
	id: number;
	name: string;
	photoFileId: string;
	description: string;
	totalPossibleKits: number;
	status: string;
	allStorages: string[];
	containsClothing: boolean;
	possibleKits: PossibleKitsItemResponse[];
	compounds: CompoundItem[];
	minAmounts: MerchKitMinAmount[];
	storageAvailabilityStatuses: StorageAvailabilityStatusItem[];
}

export interface MerchKitsGridRow {
	id: number;
	name: string;
	status: string;
	compound: string;
	description: string;
	storage: string[];
	size: string[];
	photoFileId: string;
	isClothing: boolean;
	itemRowHeight: number;
	totalPossibleKits: number;
	handleEdit: (itemId?: string | number) => () => void;
	handleTransactionAdd: (kitId?: string | number) => () => void;
	possibleKits: PossibleKitsItemResponse[];
	storageAvailabilityStatuses: StorageAvailabilityStatusItem[];
	expanded?: boolean;
	color: string;
}

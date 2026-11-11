import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import {
	MerchElementStatus,
	MinAmountItem,
} from '@/forms/MerchElementForm/types';
import { OdataParams } from '@/types/common';

export interface ElementsTabColumnsPropsFilters {
	idFilter: string;
	nameFilter: string;
	storageFilter: string[];
	statusFilter: string[];
	supplierFilter: string;
	descriptionFilter: string;
}

export interface ElementsTabColumnsProps {
	filters: ElementsTabColumnsPropsFilters;
	setFilters: MerchElementsGridFilterSetters;
	allMerchStorages: Array<SelectionModalOption>;
	allMerchStatuses: Array<SelectionModalOption>;
	allMerchElementsNames: string[];
}

export interface BodyFilters {
	name: string | null;
	elementId: string | null;
	storagesIds: string[];
	status: string[];
	supplier: string | null;
	description: string | null;
	storageSortAscending: boolean | null;
	minAmountSortAscending: boolean | null;
}

export interface MerchElementsItemEntity {
	id: number;
	name: string;
	supplier: string | null;
	description: string | null;
	photoFileId: string | null;
	isClothing: boolean | null;
	sizeAmounts: MinAmountItem[];
	status: MerchElementStatus;
	isPartOfAvailableKit: boolean | null;
}

export interface GetMerchElementsQueryParams
	extends OdataParams<ElementsTabColumnsPropsFilters> {}

export interface MerchElementsGridRow {
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
	odpItemSizeMinAmounts: MinAmountItemResponse[];
	expanded?: boolean;
	color: string;
}

export interface MinAmountItemResponse {
	minAmount: number;
	sizeId: string;
	sizeOrderWeight: number;
	warehouseId: number;
	warehouseLocation: string;
}

export interface MerchElement {
	id: number;
	name: string;
	supplier: string;
	status: string;
	description: string;
	photoFileId: string;
	isClothing: boolean;
	odpItemSizeMinAmounts: MinAmountItemResponse[];
}

export enum MerchElementsFiltersActionKeys {
	id = 'id',
	name = 'name',
	status = 'status',
	storage = 'storage',
	supplier = 'supplier',
	description = 'description',
	set = 'set',
	clear = 'clear',
}

export type MerchElementsGridFilterSetters = {
	id: (id: string) => void;
	name: (name: string) => void;
	status: (status: string[]) => void;
	storage: (storage: string[]) => void;
	supplier: (supplier: string) => void;
	description: (description: string) => void;
	set: (filters: ElementsTabColumnsPropsFilters) => void;
	clear: VoidFunction;
};

export type MerchElementsFiltersAction = {
	type: MerchElementsFiltersActionKeys;
	payload?: string | string[] | ElementsTabColumnsPropsFilters;
};

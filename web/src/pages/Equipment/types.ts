import { FromToProps } from '@/components/Grid/modals/types';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';

export interface EquipmentReducerType {
	type: string;
	payload?:
		| string
		| string[]
		| FromToProps<string>
		| FromToProps<Date>
		| EquipmentFilters;
}

export enum EquipmentActionType {
	ID = 'ID',
	RESET = 'RESET',
	AVALIABILITY = 'AVALIABILITY',
	SERIAL_NUMBER = 'SERIAL_NUMBER',
	INVENTORY_NUMBER = 'INVENTORY_NUMBER',
	INVOICE_NUMBER = 'INVOICE_NUMBER',
	NAME = 'NAME',
	TYPE = 'TYPE',
	USER = 'USER',
	DEPARTMENTS = 'DEPARTMENTS',
	EQUIPMENT_CHARACTERISTICS = 'EQUIPMENT_CHARACTERISTICS',
	PURCHASE_PLACE = 'PURCHASE_PLACE',
	REPAIR_PLACE = 'REPAIR_PLACE',
	APPROVER = 'APPROVER',
	PURCHASE_DATE = 'PURCHASE_DATE',
	GUARANTEE_DATE = 'GUARANTEE_DATE',
	LOCATION = 'LOCATION',
	NOTES = 'NOTES',
	PRICE = 'PRICE',
	STORAGE = 'STORAGE',
}

export interface EquipmentFilters {
	id: string;
	availability: string[];
	serialNumber: string;
	inventoryNumber: string;
	invoiceNumber: string;
	name: string;
	equipmentTypeId: string[];
	approver: string;
	departments: string[];
	user: string;
	equipmentCharacteristics: string;
	purchasePlace: string;
	purchaseDate: FromToProps<Date>;
	guaranteeDate: FromToProps<Date>;
	location: string[];
	storage: string[];
	notes: string;
	price: FromToProps<string>;
	repairPlace: string;
}

export interface SetFiltersType {
	id: (payload: string) => void;
	reset: (payload: EquipmentFilters) => void;
	avaliability: (payload?: string[]) => void;
	serialNumber: (payload: string) => void;
	inventoryNumber: (payload: string) => void;
	invoiceNumber: (payload: string) => void;
	name: (payload: string) => void;
	typeId: (payload?: string[]) => void;
	user: (payload?: string) => void;
	departments: (payload?: string[]) => void;
	equipmentCharacteristics: (payload: string) => void;
	purchasePlace: (payload: string) => void;
	approver: (payload?: string) => void;
	purchaseDate: (payload: FromToProps<Date>) => void;
	guaranteeDate: (payload: FromToProps<Date>) => void;
	location: (payload?: string[]) => void;
	notes: (payload: string) => void;
	price: (payload: FromToProps<string>) => void;
	storage: (payload?: string[]) => void;
	repairPlace: (payload: string) => void;
}

export interface EquipmentOptions {
	users: string[];
	equipmentTypes: Array<SelectionModalOption>;
	approvers: string[];
	equipmentLocations: Array<SelectionModalOption>;
	departments: Array<SelectionModalOption>;
	availability: Array<SelectionModalOption>;
	storage: Array<SelectionModalOption>;
}

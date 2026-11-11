import { Dispatch, SetStateAction } from 'react';

import { FromToProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { UserEquipmentGridRow } from '@/types/equipment';

export type UserEquipmentGridConstants<T> = {
	[key in keyof UserEquipmentGridRow]: T;
};

export type UserEquipmentGridColumnsNames = {
	key: keyof UserEquipmentGridRow;
	name: string;
};

export enum UserEquipmentFilterActions {
	RESET = 'reset',
	EQUIPMENT_NAME = 'equipmentName',
	EQUIPMENT_TYPE = 'equipmentType',
	SERIAL_NUMBER = 'serialNumber',
	INVENTORY_NUMBER = 'inventoryNumber',
	ISSUE_DATE = 'issueDate',
	RETURN_DATE = 'returnDate',
	IS_ASSIGNED = 'isAssigned',
}

export interface UserEquipmentFilters {
	[UserEquipmentFilterActions.EQUIPMENT_NAME]: string;
	[UserEquipmentFilterActions.EQUIPMENT_TYPE]: string[];
	[UserEquipmentFilterActions.SERIAL_NUMBER]: string;
	[UserEquipmentFilterActions.INVENTORY_NUMBER]: string;
	[UserEquipmentFilterActions.ISSUE_DATE]: FromToProps<Date>;
	[UserEquipmentFilterActions.RETURN_DATE]: FromToProps<Date>;
	[UserEquipmentFilterActions.IS_ASSIGNED]: string[];
}

export type UserEquipmentReducerAction = {
	[K in UserEquipmentFilterActions]: {
		type: K;
		payload: K extends keyof UserEquipmentFilters
			? UserEquipmentFilters[K]
			: UserEquipmentFilters;
	};
}[UserEquipmentFilterActions];

export type UserEquipmentFiltersHandlers = {
	[K in UserEquipmentFilterActions]: K extends keyof UserEquipmentFilters
		? Dispatch<SetStateAction<UserEquipmentFilters[K]>>
		: VoidFunction;
};

export type UserEquipmentFiltersReducer = [
	UserEquipmentFilters,
	UserEquipmentFiltersHandlers
];

export interface UserEquipmentFiltersOptions {
	equipmentTypes: Array<SelectionModalOption>;
	assignStatuses: Array<SelectionModalOption>;
}

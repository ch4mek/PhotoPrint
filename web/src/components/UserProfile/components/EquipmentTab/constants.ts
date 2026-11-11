import { fromToDateObject } from '@/constants/defaultValues';
import { AssignStatus } from '@/constants/equipment';
import { GridColumnWidth } from '@/types/grid';

import {
	UserEquipmentFilters,
	UserEquipmentGridColumnsNames,
	UserEquipmentGridConstants,
} from './types';

export const EDIT_COLUMN_WIDTH = 44;

export const defaultUserEquipmentFilters: UserEquipmentFilters = {
	equipmentName: '',
	equipmentType: [],
	serialNumber: '',
	inventoryNumber: '',
	issueDate: fromToDateObject,
	returnDate: fromToDateObject,
	isAssigned: [AssignStatus.Assigned],
};

export const equipmentGridColumns: UserEquipmentGridConstants<UserEquipmentGridColumnsNames> =
	{
		equipmentName: {
			key: 'equipmentName',
			name: 'equipmentName',
		},
		equipmentTypeName: {
			key: 'equipmentTypeName',
			name: 'type',
		},
		serialNumber: {
			key: 'serialNumber',
			name: 'serialNumber',
		},
		inventoryNumber: {
			key: 'inventoryNumber',
			name: 'inventoryNumber',
		},
		issueDate: {
			key: 'issueDate',
			name: 'issueDate',
		},
		returnDate: {
			key: 'returnDate',
			name: 'returnDate',
		},
		isAssigned: {
			key: 'isAssigned',
			name: 'status',
		},
	};

export const equipmentGridColumnWidths: UserEquipmentGridConstants<GridColumnWidth> =
	{
		equipmentName: {
			width: 200,
			fixed: false,
		},
		equipmentTypeName: {
			width: 138,
			fixed: false,
		},
		serialNumber: {
			width: 200,
			fixed: false,
		},
		inventoryNumber: {
			width: 150,
			fixed: true,
		},
		issueDate: {
			width: 117,
			fixed: true,
		},
		returnDate: {
			width: 117,
			fixed: true,
		},
		isAssigned: {
			width: 88,
			fixed: true,
		},
	};

export const equipmentDesktopGridColumnWidths: UserEquipmentGridConstants<GridColumnWidth> =
	{
		...equipmentGridColumnWidths,
		equipmentTypeName: {
			width: 200,
			fixed: false,
		},
	};

export const RESET_FILTER_COLUMN_INDEX =
	Object.values(defaultUserEquipmentFilters).length + 2;

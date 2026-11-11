import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { fromToDateObject } from '@/constants/defaultValues';
import { AssignStatus, assignStatusLabel } from '@/constants/equipment';
import { GridColumnWidth } from '@/types/grid';

import {
	UserEquipmentFilters,
	UserEquipmentGridColumnsNames,
	UserEquipmentGridConstants,
} from './types';

export const EDIT_COLUMN_WIDTH = 44;

export const assignStatusOptions: Array<SelectionModalOption> = [
	{
		id: AssignStatus.Assigned,
		description: assignStatusLabel[AssignStatus.Assigned],
	},
	{
		id: AssignStatus.Returned,
		description: assignStatusLabel[AssignStatus.Returned],
	},
];

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
			name: 'Equipment name',
		},
		equipmentTypeName: {
			key: 'equipmentTypeName',
			name: 'Type',
		},
		serialNumber: {
			key: 'serialNumber',
			name: 'Serial number',
		},
		inventoryNumber: {
			key: 'inventoryNumber',
			name: 'Inventory number',
		},
		issueDate: {
			key: 'issueDate',
			name: 'Issue date',
		},
		returnDate: {
			key: 'returnDate',
			name: 'Return date',
		},
		isAssigned: {
			key: 'isAssigned',
			name: 'Status',
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

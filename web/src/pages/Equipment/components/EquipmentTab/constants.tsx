import { EquipmentFilters } from '../../types';

import { DEFAULT_ID_COLUMN_CONFIG } from '@/components/Grid/constants';
import {
	fromToDateObject,
	fromToCurrencyObject,
} from '@/constants/defaultValues';
import { isHeadOrGroupManager, Roles } from '@/constants/roles';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { UserAccessClaims, UserClaims } from '@/types/claims';
import { GridColumnWidthsConfiguration } from '@/types/grid';

export const DEFAULT_EQUIPMENT_FILTERS: EquipmentFilters = {
	id: '',
	availability: [],
	equipmentTypeId: [],
	serialNumber: '',
	inventoryNumber: '',
	invoiceNumber: '',
	name: '',
	approver: '',
	user: '',
	equipmentCharacteristics: '',
	purchasePlace: '',
	notes: '',
	departments: [],
	guaranteeDate: fromToDateObject,
	location: [],
	storage: [],
	repairPlace: '',
	price: fromToCurrencyObject,
	purchaseDate: fromToDateObject,
};

export const getEquipmentColumns = (
	role: Roles,
	claims?: UserAccessClaims
): GridColumnWidthsConfiguration => {
	const hasManageAllEquipmentClaim = claims
		? checkIsHasClaims([UserClaims.ManageAllEquipment], claims)
		: false;
	const shouldHideColumns =
		isHeadOrGroupManager(role) && !hasManageAllEquipmentClaim;

	return {
		id: DEFAULT_ID_COLUMN_CONFIG,
		availability: 117,
		invoiceNumber: 145,
		serialNumber: 145,
		inventoryNumber: 150,
		name: 145,
		type: 100,
		priceUSD: shouldHideColumns ? 0 : 100,
		user: 175,
		department: shouldHideColumns ? 0 : 123,
		equipmentCharacteristics: 200,
		purchasePlace: shouldHideColumns ? 0 : 180,
		approver: 130,
		purchaseDate: 137,
		storage: 106,
		guaranteeDate: 143,
		location: 105,
		repairPlace: 181,
		jiraTask: 200,
		comment: 200 + (shouldHideColumns ? 0 : 100),
	};
};

export const EQUIPMENT_GRID_COLUMNS = {
	id: {
		key: 'id',
		name: 'Id',
	},
	availability: {
		key: 'status',
		name: 'Availability',
	},
	invoiceNumber: {
		key: 'invoiceNumber',
		name: 'Invoice number',
	},
	serialNumber: {
		key: 'serialNumber',
		name: 'Serial number',
	},
	inventoryNumber: {
		key: 'inventoryNumber',
		name: 'Inventory number',
	},
	name: {
		key: 'name',
		name: 'Name',
	},
	equipmentTypeId: {
		key: 'equipmentTypeId',
		name: 'Type',
	},
	purchasePriceUsd: {
		key: 'purchasePriceUsd',
		name: 'Price, $',
	},
	activeHolderFullName: {
		key: 'activeHolderFullName',
		name: 'User',
	},
	activeHolderDepartmentId: {
		key: 'activeHolderDepartmentId',
		name: 'Department',
	},
	equipmentCharacteristics: {
		key: 'equipmentCharacteristics',
		name: 'Equipment characteristics',
	},
	purchasePlace: {
		key: 'purchasePlace',
		name: 'Purchase place',
	},
	approverFullName: {
		key: 'approverFullName',
		name: 'Approver',
	},
	purchaseDate: {
		key: 'purchaseDate',
		name: 'Purchase date',
	},
	storage: {
		key: 'storage',
		name: 'Storage',
	},
	guaranteeDate: {
		key: 'guaranteeDate',
		name: 'Guarantee date',
	},
	location: {
		key: 'location',
		name: 'Location',
	},
	repairPlace: {
		key: 'repairPlace',
		name: 'Repair place',
	},
	jiraTask: {
		key: 'jiraTask',
		name: 'Jira task',
	},
	comment: {
		key: 'comment',
		name: 'Notes',
	},
};

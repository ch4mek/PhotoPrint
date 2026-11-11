import { GridCellDefaultProps } from '@/components/Grid';
import { Currency } from '@/constants/currency';
import { Departments } from '@/constants/departments';
import { StatusEquipment } from '@/forms/EquipmentForm/constants';
import { Dayjs } from '@/lib/dayjs';
import { ByPageResponse } from '@/queries/types';

import { EquipmentType, LocationTypeId } from './configuration';

export interface ActiveHolder {
	id?: number;
	fullName: string;
	departmentId?: Departments;
	issueDate?: Dayjs | string;
	returnDate?: Dayjs;
}

export interface Approver {
	id: number;
	fullName: string;
}

export interface EquipmentItemResponse {
	id: string;
	name: string;
	equipmentTypeId: string;
	serialNumber: string;
	inventoryNumber: string;
	invoiceNumber: string;
	purchasePrice: number;
	purchaseCurrency: Currency;
	purchasePriceAndCurrency: string;
	purchasePriceUsd: number;
	purchasePriceUsdCurrency?: Currency;
	purchaseDate: Dayjs;
	purchasePlace: string;
	guaranteeDate: Dayjs;
	characteristics: string;
	jiraTask: string;
	location: string;
	comment: string;
	approver: Approver;
	storage: LocationTypeId;
	activeHolder: ActiveHolder | null;
	type: EquipmentType;
	brokenStatus: string;
	repairPlace: string;
	status: StatusEquipment;
}

export interface EquipmentItemRequest
	extends Partial<Omit<EquipmentItemResponse, 'approver' | 'status'>> {
	brokenStatus: string;
	approverId: number;
	purchasePriceUsdCurrency?: Currency;
	activeHolderRequest?: {
		userId: number;
		issueDate: Dayjs;
		returnDate?: Dayjs;
	};
}

export interface EquipmentHistoryItem {
	approverId: number;
	approverFullName: string;
	equipmentHistoryId: number;
	id: number;
	fullName: string;
	departmentId: string;
	resourceGroupId: number | null;
	issueDate: Dayjs;
	returnDate: Dayjs;
	firstName: string;
	lastName: string;
	approverLastName: string;
	approverFirstName: string;
}

export interface AssignmentsHistoryItem {
	id: string;
	userId: string;
	equipmentId: string;
	userDepartmentId: string;
	userResourceGroupId: null;
	userFirstName: string;
	userLastName: string;
	userFullName: string;
	equipmentName: string;
	equipmentTypeId: string;
	serialNumber: string;
	inventoryNumber: string;
	issueDate: Dayjs;
	returnDate: Dayjs;
	isAssigned: boolean;
}

export interface EquipmentGridRow extends GridCellDefaultProps {
	id: string;
	serialNumber: string;
	inventoryNumber: string;
	invoiceNumber: string;
	status: string;
	name: string;
	equipmentTypeId: string;
	purchasePriceUsd: string;
	activeHolderFullName: string;
	activeHolderDepartmentId: string;
	equipmentCharacteristics: string;
	purchasePlace: string;
	approverFullName: string;
	purchaseDate: string;
	guaranteeDate: string;
	location: string;
	comment: string;
	jiraTask: string;
}

export interface AssignmentHistoryGridRow extends GridCellDefaultProps {
	userId: string;
	equipmentId: string;
	userFullName: string;
	isAssigned: boolean;
	userDepartmentId: string;
	equipmentName: string;
	equipmentTypeId: string;
	serialNumber: string;
	inventoryNumber: string;
	issueDate: string;
	returnDate: string;
}

export interface EquipmentHistoryResponse
	extends ByPageResponse<EquipmentHistoryItem> {}

export interface UserEquipmentItem {
	equipmentTypeId: string;
	equipmentTypeName: string;
	equipmentName: string;
	serialNumber: string;
	inventoryNumber: string;
	issueDate: string;
	returnDate: string;
	isAssigned: string;
}

export type UserEquipmentResponse = ByPageResponse<UserEquipmentItem>;

export type UserEquipmentGridRow = GridCellDefaultProps &
	Omit<UserEquipmentItem, 'equipmentTypeId'>;

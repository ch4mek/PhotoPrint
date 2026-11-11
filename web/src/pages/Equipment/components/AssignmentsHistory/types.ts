import { FromToProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';

export interface AssignmentHistoryFilters {
	userId: string;
	equipmentId: string;
	userFullName: string;
	isAssigned: string[];
	departments: string[];
	equipmentName: string;
	type: string[];
	serialNumber: string;
	inventoryNumber: string;
	issueDate: FromToProps<Date>;
	returnDate: FromToProps<Date>;
}

export enum AssignmentsHistoryActionType {
	RESET = 'RESET',
	USER_ID = 'USER_ID',
	EQUIPMENT_ID = 'EQUIPMENT_ID',
	USER_FULL_NAME = 'USER_FULL_NAME',
	IS_ASSIGNED = 'IS_ASSIGNED',
	DEPARTMENTS = 'DEPARTMENTS',
	EQUIPMENT_NAME = 'EQUIPMENT_NAME',
	EQUIPMENT_TYPE = 'EQUIPMENT_TYPE',
	SERIAL_NUMBER = 'SERIAL_NUMBER',
	INVENTORY_NUMBER = 'INVENTORY_NUMBER',
	ISSUE_DATE = 'ISSUE_DATE',
	RETURN_DATE = 'RETURN_DATE',
}

export interface AssignmentsHistoryReducerType {
	type: string;
	payload?: string | string[] | FromToProps<Date> | AssignmentHistoryFilters;
}

export interface SetAssignmentsHistoryFiltersType {
	reset: (payload: AssignmentHistoryFilters) => void;
	userId: (payload: string) => void;
	equipmentId: (payload: string) => void;
	userFullName: (payload?: string) => void;
	availability: (payload?: string[]) => void;
	serialNumber: (payload: string) => void;
	inventoryNumber: (payload: string) => void;
	name: (payload: string) => void;
	typeId: (payload?: string[]) => void;
	departments: (payload?: string[]) => void;
	issueDate: (payload: FromToProps<Date>) => void;
	returnDate: (payload: FromToProps<Date>) => void;
}

export interface AssignmentHistoryOptions {
	users: string[];
	equipmentTypes: Array<SelectionModalOption>;
	departments: Array<SelectionModalOption>;
}

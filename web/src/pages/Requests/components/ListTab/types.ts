import { FromToProps, GridCellDefaultProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { AbsenceStatusEntity } from '@/types/absences';
import { RoleType } from '@/types/configuration';

export interface RequestFilterType {
	id: string | number;
	userId: string;
	userFullName: string;
	userPosition: string[];
	userDepartment: string[];
	inquiryTemplateName: string;
	dateFrom: FromToProps<Date>;
	dateTo: FromToProps<Date>;
	status: string[];
	createdAtUtc: FromToProps<Date>;
	notes: string;
}

export interface FormParams {
	inquiryId: string | number | null;
}

export interface OptionsProps {
	users: string[];
	positions: RoleType[];
	departments: SelectionModalOption[];
	absenceNames: string[];
	statuses: SelectionModalOption[];
	onFormOpen: (params: FormParams) => void;
}

export interface RequestGridRow extends GridCellDefaultProps {
	id: number;
	userId: number;
	userFullName: string;
	userPosition: string;
	dateFrom: string;
	dateTo: string | null;
	quantity: string;
	userDepartmentId: string;
	status: AbsenceStatusEntity;
	createdAtUtc: string;
	notes: string | null;
	inquiryType: string;
	inquiryTemplateName: string;
	managedByRoleRequest: boolean;
}

export enum RequestFiltersNames {
	USER_ID = 'userId',
	USER = 'userFullName',
	USER_POSITION = 'userPosition',
	ABSENCE_NAME = 'inquiryTemplateName',
	USER_DEPARTMENT = 'userDepartment',
	DATE_FROM = 'dateFrom',
	DATE_TO = 'dateTo',
	STATUS = 'status',
	CREATED = 'createdAtUtc',
	NOTES = 'notes',
	RESET = 'reset',
}

export interface RequestAction {
	type: string;
	payload?:
		| string[]
		| string
		| FromToProps<string>
		| FromToProps<Date>
		| RequestFilterType;
}

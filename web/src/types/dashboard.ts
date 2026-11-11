import { AbsenceType } from '@/types/absences';

export interface EventsWidgetRow {
	id: string;
	userId: number;
	fullName: string;
	eventType: string;
	eventDate: string;
}

export enum EmployeeEventType {
	EmploymentAnniversary = 'EmploymentAnniversary',
	Birthday = 'Birthday',
}

export interface EmployeeEvent {
	userId: number;
	fullName: string;
	departmentId: string | null;
	resourceGroupId: number | null;
	eventType: EmployeeEventType;
	eventCelebrationDate: string;
	originalEventDate: string;
}

export interface RequestsWidgetRow {
	id: number | string;
	userId: number;
	userFullName: string;
	inquiryId: string | number;
	inquiryType: string;
	inquiryTemplateName: string;
	dateFrom: string;
	canUserBeRetrieved: boolean;
}

export interface RequestsWidgetResponse {
	userId: number;
	inquiryId: number;
	inquiryTemplateId: number;
	inquiryTemplateName: string;
	userFullName: string;
	inquiryType: AbsenceType;
	dateFrom: string;
	canUserInfoBeRetrievedByCurrentUser: boolean;
}

export type ContractExpirationItem = {
	id: number;
	userId: number;
	userFullName: string;
	contractTo: string | null;
};

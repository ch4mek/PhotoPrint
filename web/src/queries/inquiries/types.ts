import { Dayjs } from 'dayjs';

import { DocumentResponse } from '@/types/document';

export interface Inquiry {
	id: number;
	inquiryUserId: number;
	inquiryTemplateId: number;
	status: string;
	notes?: string;
	dateFrom: string;
	dateTo: string;
	totalDays: number;
	totalAccountantDays: number;
	compensationAmount: number;
	createdById: number;
	createdAtUtc: string;
	updatedById: number;
	updatedAtUtc: string;
	userInquiryDates: Array<{
		id: number;
		date: string;
		requestedDays: number;
		dateType: string;
		isExcludedFromInquiry: boolean;
		excludedDayName?: string;
	}>;
	userInquiryAccepts: Array<{
		id: number;
		status: string;
		acceptUser?: User;
		acceptedByUser?: User;
		rejectedByUser?: User;
		acceptedAtUtc?: string;
		rejectedAtUtc?: string;
	}>;
}

interface User {
	id: number;
	firstName: string;
	lastName: string;
	email: string;
	departmentId: string;
	countryId: number;
	avatarFieldId?: string;
	position?: string;
}

export interface RequestSelectedParam {
	dateFrom: string | Date | Dayjs | null;
	dateTo: string | Date | Dayjs | null;
	templateId: string | number;
	userId: string | number;
}

export interface RequestSelectedDays {
	selectedDays: number;
}

export interface RequestAddData {
	notes: string | null;
	inquiryTemplateId: string | number;
	agreementTemplateId?: number | null;
	dateFrom: string;
	dateTo: string;
	hours: string | null;
	userId: number;
	files: DocumentResponse[];
}

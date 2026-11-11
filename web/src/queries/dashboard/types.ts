import { TagTypes } from '@/components/DateTag/types';
import { Roles } from '@/constants/roles';
import { IncludedDepartments } from '@/pages/Dashboard/types';
import { AbsenceType } from '@/types/absences';

export interface UseGetDashboardParams {
	departmentId: IncludedDepartments | null;
}

export type GetPerformanceReviewResponse = {
	id: number;
	userId: number;
	userFirstName: string;
	userLastName: string;
	userFirstNameRu: string;
	userLastNameRu: string;
	date: string;
	performanceReviewDateType: TagTypes;
}[];

export type GetProbationResponse = {
	id: number;
	userId: number;
	userFullName: string;
	date: string;
}[];

export type GetTodaysInterviewsResponse = {
	id: number;
	userId: number;
	userFullName: string;
	userPosition: Roles;
	userDepartmentId: string;
	specialization: string;
	userInterviewDate: string;
	userInterviewFromUtc: string;
	status: string;
}[];

export type GetApprovedAbsencesResponse = {
	userId: number;
	inquiryId: number;
	userFullName: string;
	userPosition: Roles;
	inquiryType: AbsenceType;
	inquiryTemplateName: string;
}[];

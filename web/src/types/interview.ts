import { Resources } from 'i18next';

import { ByPageResponse } from '../queries/types';

import { Departments } from '@/constants/departments';
import { Roles } from '@/constants/roles';
import { ThemeMode } from '@/theme';

export interface InterviewData {
	id: number;
	userId: number;
	userFirstName: string;
	userLastName: string;
	userFullName: string;
	userFirstNameRu: string | null;
	userLastNameRu: string | null;
	userFullNameRu: string | null;
	userPosition: Roles;
	userDepartmentId: Departments;
	userResourceGroupId: string | null;
	specialization: string;
	userInterviewDate: string | null;
	userInterviewFromUtc: string | null;
	clientName: string | null;
	status: InterviewStatus | null;
	statusOrderWeight: number;
	interviewType: InterviewType | null;
	interviewTypeOrderWeight: number;
	salesResponsibleUserId: number | null;
	salesResponsibleUserFirstName: string | null;
	salesResponsibleUserLastName: string | null;
	salesResponsibleUserFullName: string | null;
	salesResponsibleUserFirstNameRu: string | null;
	salesResponsibleUserLastNameRu: string | null;
	salesResponsibleUserFullNameRu: string | null;
	jiraTask: string | null;
	notes: string | null;
	reasonForRefusal: string | null;
	groupId: string;
	isUserActive: boolean;
}

interface InterviewGroup {
	main: InterviewData;
	stages: InterviewData[];
}

export type GetInterviewDataResponse = ByPageResponse<InterviewGroup>;

export enum InterviewType {
	Prescreen = 'Prescreen',
	Tech = 'Tech',
	Final = 'Final',
	Other = 'Other',
}

export enum InterviewStatus {
	WaitingForInterview = 'WaitingForInterview',
	WaitingForResult = 'WaitingForResult',
	Failed = 'Failed',
	Success = 'Success',
	Started = 'Started',
	NotStarted = 'NotStarted',
	Canceled = 'Canceled',
	NoFeedback = 'NoFeedback',
	NoNextStepScheduled = 'NoNextStepScheduled',
}

export interface LookupSaleManagersResponse {
	id: number;
	firstName: string;
	lastName: string;
	firstNameRu: string | null;
	lastNameRu: string | null;
}

export type InterviewStatusNameColorKey =
	keyof Resources['dashboard']['interview']['tagNames'];
export type InterviewStatusColorInfo = Record<ThemeMode, string> & {
	nameColor: InterviewStatusNameColorKey;
};

export interface CreateUpdateInterviewTemplate {
	userId: number;
	specialization: string;
	client: string | null;
	jiraTask: string | null;
	type: InterviewType | null;
	status: InterviewStatus | null;
	interviewDate: string | null;
	interviewTimeFromUtc: string | null;
	saleManagerId: number | null;
	reasonForRefusal: string | null;
	notes: string | null;
}

export interface UpdateInterview extends CreateUpdateInterviewTemplate {
	interviewId: number;
}

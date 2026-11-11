import { TagTypes } from '@/components/DateTag/types';

import { AbsenceRequestStatus, AbsenceType } from './absences';

export interface HomeManagerUsersResponse {
	page: number;
	pageSize: number;
	totalCount: number;
	items: {
		userId: number;
		userName: string;
		color: string;
		countOfProjects: number;
		totalTrackedInMinutes: number;
	}[];
}

export enum UpcomingEventType {
	DevelopmentPlan = 'Development plan',
	PerformanceReview = 'Performance review',
	Contract = 'Contract',
	Absence = 'Absence',
}

export interface UpcomingEvent {
	endDate: string | null;
	name: string | null;
	performanceReviewDateType: TagTypes | null;
	startDate: string | null;
	status: AbsenceRequestStatus | null;
	type: UpcomingEventType;
	inquiryType: AbsenceType | null;
	inquiryId: number | null;
}

export interface UserBirthday {
	avatarFileId?: string | null;
	department: string | null;
	lastName: string;
	firstName: string;
}

export interface AllBirthdaysResponseUserBirthday {
	day: number;
	users: UserBirthday[];
}

export interface AllBirthdaysResponseItem {
	month: number;
	birthdaysCount: number;
	days: AllBirthdaysResponseUserBirthday[];
}

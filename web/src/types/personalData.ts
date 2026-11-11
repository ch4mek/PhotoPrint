import { TagTypes } from '@/components/DateTag/types';
import { Roles } from '@/constants/roles';
import { emailFormFields } from '@/constants/user';

export enum MilitaryServiceStatus {
	Fit = 'Fit',
	Unfit = 'Unfit',
	Served = 'Served',
}

export enum GraduateJobPlacement {
	Yes = 'Yes',
	No = 'No',
}

export interface UserNameValues {
	firstName: string;
	lastName: string;
	firstNameRu: string | null;
	lastNameRu: string | null;
	patronymicRu: string | null;
}

export interface PersonalDataResponse extends UserNameValues {
	fullName: string;
	fullNameRu: string | null;
	email: string;
	personalEmail: string | null;
	personalEmailSecond: string | null;
	personalPhone: string | null;
	seniority: string | null;
	primarySpecialization: string | null;
	softSkillAverageSeniority: string | null;
	englishLevel: string | null;
	comments: string | null;
	avatarFileId: string | null;
	departmentId: string | null;
	departmentFullName: string | null;
	resourceGroupId: number | null;
	resourceGroupName: string | null;
	upcomingPerfomanceReview: string | null;
	performanceReviewDateStatus: TagTypes | null;
	location: string;
	currentLocation: string | null;
	registeredLocation: string;
	dateOfBirth: string | null;
	telegram: string | null;
	needProject: boolean;
	registeredLocationId: number | null;
	militaryServiceStatus: MilitaryServiceStatus | null;
	isGraduatedJobPlacement: boolean | null;
	linkedIn: string | null;
	position: Roles;
}

export type EmailFieldName =
	(typeof emailFormFields)[keyof typeof emailFormFields]['name'];

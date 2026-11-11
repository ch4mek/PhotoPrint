import { TagTypes } from '@/components/DateTag/types';
import { GridCellDefaultProps } from '@/components/Grid';
import { Dayjs } from '@/lib/dayjs';
import { ByPageResponse } from '@/queries/types';
import { ConversationDocument } from '@/types/document';

import {
	SeniorityLevelType,
	SimpleConfigurableConfigurationItem,
} from './configuration';
import { FormAutoselectOption } from './forms';

export interface TeamboardResponse {
	teamBoardUsers: ByPageResponse<Teamboard>;
}

interface NonPrimarySpecializations {
	id: number;
	userId: number;
	name: string;
	orderWeight: number;
	seniorityLevelId: string;
	seniorityLevel: string;
	isVisibleSpecialization: true;
	isActive: boolean;
	createdAtUtc: Dayjs;
}

export interface Teamboard {
	id: number;
	firstName: string;
	lastName: string;
	fullName: string;
	careerStartedAt: Dayjs | null;
	profExperienceInMonths: number;
	englishLevelId: string | null;
	englishLevelName: string | null;
	englishLevelOrderWeight: string | null;
	seniorityLevelId: string | null;
	seniorityLevel: string | null;
	seniorityLevelOrderWeight: string | null;
	primarySpecializationId: string | null;
	primarySpecializationName: string | null;
	departmentId: string | null;
	departmentFullName: string | null;
	departmentOrderWeight: string | null;
	resourceGroupName: string | null;
	currentLocation: string | null;
	position: string;
	positionOrderWeight: number;
	location: string;
	registeredLocation: string;
	comments: string | null;
	isActive: true;
	odpNonPrimarySpecializations: Array<NonPrimarySpecializations>;
}

export interface TeamboardGridRow extends GridCellDefaultProps {
	id: number;
	fullName: string;
	departmentFullName: string | null;
	resourceGroupName: string | null;
	profExperienceInMonths: number | null;
	englishLevelOrderWeight: string | null;
	seniorityLevelOrderWeight: string | null;
	primarySpecializationName: string[];
	currentLocation: string | null;
	location: string;
	registeredLocation: string;
	comments: string | null;
	positionOrderWeight?: string;
}

export interface AssignedUserSoftSkill {
	softSkillId: number;
	softSkillName: string;
	seniorityLevelId: string | null;
	seniorityLevelName: string | null;
	softSkillSeniorityOrderWeight: string;
	snapshotDate: string | null;
}

export interface UserHardSkillById {
	id: number;
	userId: number;
	isPrimary: boolean;
	seniorityLevel: string;
	specializationTemplate: string;
}

interface ConversationDocumentLink {
	id: number;
	link: string;
}

export interface UserConversation {
	id: number;
	date: string;
	theses: string;
	employeeTheses: string;
	agreements: string;
	userId: number;
	userDepartmentId: string | null;
	userFirstName: string;
	userLastName: string;
	userFullName: string;
	supervisorId: number;
	supervisorFullName: string;
	supervisorFirstName: string;
	supervisorLastName: string;
	conversationResult: string;
	conversationType: string;
	performanceReviewDate: string;
	passed: boolean;
	status: string | null;
	odpConversationFiles: ConversationDocument[];
	odpConversationDocumentLinks: ConversationDocumentLink[];
}

export interface UserSupervisor {
	id: number;
	fullName: string;
}

export interface UserFeedback {
	id: number;
	externalFeedbackDate: string;
	feedback: string;
	actionsTodo: string;
	externalFeedbackType: string;
	projectId: number;
	projectName: string;
	clientName: string;
	conversationId: number;
	userId: number;
	userDepartmentId: string;
	userFirstName: string;
	userLastName: string;
	userFullName: string;
	reviewer: string;
}

export enum TeamboardTableMode {
	Section = 'Section',
	Tab = 'Tab',
	PopUp = 'Popup',
	Unknown = 'Unknown',
}

export interface UserSalary {
	id: number;
	userId: number;
	salaryFrom: string;
	salaryUsd: number;
	isCurrent: boolean;
}

export interface UserPayslips {
	fileId: string;
	fileName: string;
	reportDate: string;
	userId: number;
	createdAtUtc: string;
}

export interface HardSkill {
	id: number;
	name: string;
	seniorityLevelId: string;
	seniorityLevelName: string;
	isActive: boolean;
}

export interface UserSpecialization {
	id: number;
	seniorityLevelId: string;
	seniorityLevelName: string;
	wasPrimary: boolean;
	specializationTemplateId: number;
	specializationTemplateName: string;
	hardSkills: HardSkill[];
}

export interface SpecializationSnapshot {
	id: number;
	userId: number;
	snapshotDate: string;
	userSpecializations: UserSpecialization[];
}

export interface UserSpecializationInfo {
	id: number;
	seniorityLevelId: string;
	seniorityLevelName: string;
	isPrimary: boolean;
	specializationTemplateId: number;
	specializationTemplateName: string;
	specializationSnapshots: SpecializationSnapshot[];
}

export interface FormFullHardSkillInfo {
	seniority: SeniorityLevelType | null;
	info: SimpleConfigurableConfigurationItem<number>;
}

export interface FormSpecialization {
	id: number;
	name: string;
	hardSkills: Array<FormFullHardSkillInfo>;
	seniority: SeniorityLevelType | null;
}

export interface UserSpecializationFormValues {
	date: Dayjs;
	specialization: FormSpecialization | null;
}

export interface UserNotAssignedSpecialization {
	specializationTemplateId: number;
	specializationTemplateName: string;
	hardSkills: Array<FormAutoselectOption>;
}

export type SpecializationToDelete =
	SimpleConfigurableConfigurationItem<number>;

export type SpecializationToPrimary =
	SimpleConfigurableConfigurationItem<number> & {
		specializationTemplateId: number;
	};

export interface UserPerformanceReviewData {
	id: number;
	performanceReviewDate: string;
	performanceReviewDateType: TagTypes;
	conversationId: number;
	theses: string;
	employeeTheses: string;
	agreements: string;
	odpConversationFiles: ConversationDocument[];
	odpConversationDocumentLinks: ConversationDocumentLink[];
}

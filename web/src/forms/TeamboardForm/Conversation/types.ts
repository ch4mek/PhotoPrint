import { UpdateUserSalary } from '../UserSalary/types';

import { Dayjs } from '@/lib/dayjs';
import { UserDevelopmentPlanRequestProps } from '@/queries/teamboard/types';
import {
	ConversationFormDocument,
	CreateConversationFileDto,
} from '@/types/document';
import { FormAutoselectOption } from '@/types/form';
import { TeamboardTableMode } from '@/types/teamboard';

export interface ConversationFormInitialValues {
	performanceReviewDate?: Dayjs | null;
	conversationType?: FormAutoselectOption<string>;
	performanceReviewId?: number;
	resultType?: FormAutoselectOption<string>;
}

export interface ConversationFormProps {
	handleClose: () => void;
	initialValues?: ConversationFormInitialValues;
	onSubmit: (
		values: UpdateConversationFormValues,
		isPassedUpdatedToTrue: boolean
	) => void;
	id?: number;
}

interface ConversationFormBaseValues {
	date: Dayjs | null;
	theses: string;
	employeeTheses: string;
	agreements: string;
	performanceReviewDate: Dayjs | null;
	files: ConversationFormDocument[];
	documentLinks: string[];
}

export interface ConversationFormValues extends ConversationFormBaseValues {
	conversationType: FormAutoselectOption<string> | null;
	conversationResult: FormAutoselectOption<string> | null;
	passed: FormAutoselectOption | null;
}

interface ConversationFormSubmittedValues
	extends Omit<ConversationFormBaseValues, 'files'> {
	conversationType: string | null;
	conversationResult: string | null;
	passed: boolean | null;
	files: ConversationFormDocument[];
}

export enum StatusOfItem {
	ADDED = 'ADDED',
	EDITED = 'EDITED',
}

export interface UserSalaryPopUp extends UpdateUserSalary {
	status: StatusOfItem;
}

export interface DevelopmentPlanPopUp {
	userId: number;
	id: number;
	startDate: string;
	deadlineDate: string | null;
	endDate?: string;
	summary: string;
	notes: string;
	status: StatusOfItem;
	conversationId?: number | undefined;
}

export interface UpdateConversationFormValues
	extends ConversationFormSubmittedValues {
	userSalaries: UserSalaryPopUp[];
	developmentPlans: DevelopmentPlanPopUp[];
	userPerformanceReviewId?: number;
}

export enum ConversationType {
	Conversation = 'Conversation',
	PerformanceReview = 'Perf. review',
	HRConversation = 'HRConversation',
}

export const CONVERSATION_TYPE_LABELS: Record<ConversationType, string> = {
	[ConversationType.Conversation]: 'Conversation',
	[ConversationType.PerformanceReview]: 'Perf. review',
	[ConversationType.HRConversation]: 'HR conversation',
};

export enum PassedStatus {
	Yes = 'Yes',
	No = 'No',
}

export enum StatusType {
	Finished = 'Finished',
	Passed = 'Passed',
}

export enum ConversationFormField {
	DATE = 'date',
	TYPE = 'conversationType',
	PERFORMANCE_DATE = 'performanceReviewDate',
	PASSED = 'passed',
	RESULT = 'conversationResult',
	THESES = 'theses',
	EMPLOYEE_THESES = 'employeeTheses',
	AGREEMENTS = 'agreements',
	FILES = 'files',
	DOCUMENT_LINKS = 'documentLinks',
}

interface ConversationSalaries {
	id: string | number | null;
	userId: string | number;
	salaryFrom: string | null;
	salaryUsd: number | null;
	conversationId: string | number | null;
	source: TeamboardTableMode;
}

interface ConversationDevelopmentPlans extends UserDevelopmentPlanRequestProps {
	source: TeamboardTableMode;
}

export interface CreateUserConversation {
	date: string | null;
	theses: string;
	employeeTheses: string;
	agreements: string;
	userId: number;
	conversationType: string;
	conversationResult: string | null;
	performanceReviewDate: string | null;
	passed: boolean | null;
	userPerformanceReviewId?: number;
	documentLinks: string[];
	files: CreateConversationFileDto[];
	salaries: ConversationSalaries[];
	developmentPlans: ConversationDevelopmentPlans[];
}

export interface UpdateUserConversation extends CreateUserConversation {
	id: number | null;
}

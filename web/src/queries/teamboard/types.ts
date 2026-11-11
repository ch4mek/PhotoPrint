import { ByPageResponse } from '../types';

import { PayslipsTableFilters } from '@/components/PayslipsTable/types';
import { SalaryTableFilters } from '@/pages/TeamBoardUser/components/SalaryTable/types';
import {
	ConversationFilter,
	FeedbackFilter,
	PerformanceReviewFilter,
} from '@/pages/TeamBoardUser/types';
import { OdataParams } from '@/types/common';
import {
	TeamboardTableMode,
	UserNotAssignedSpecialization,
} from '@/types/teamboard';

export type ConversationQueryFilter = Partial<
	ConversationFilter & { id: number }
>;

export type FeedbackQueryFilter = Partial<FeedbackFilter & { id: number }>;

export interface GetConversationQueryParams
	extends OdataParams<ConversationQueryFilter> {
	userId: number | undefined;
}

type UserSalaryQueryFilter = Partial<SalaryTableFilters & { id: number }>;
type UserPayslipsQueryFilter = Partial<PayslipsTableFilters>;

export interface GetSalaryQueryParams
	extends OdataParams<UserSalaryQueryFilter> {
	userId: number | undefined;
	mode: TeamboardTableMode;
	conversationId?: number | undefined;
}

export interface GetPayslipsQueryParams
	extends OdataParams<UserPayslipsQueryFilter> {
	userId: number | undefined;
}

export interface GetFeedbackQueryParams
	extends OdataParams<FeedbackQueryFilter> {
	userId: number | undefined;
}
export interface UserContract {
	id: number;
	userId: number;
	contractFrom: string;
	contractTo?: string | null;
	fireDate?: string | null;
	fireType: string | null;
	fireReason: string;
	fireDescription: string;
}

export interface GetUserContractsResponse
	extends ByPageResponse<UserContract> {}

export interface CreateUserContract extends Omit<UserContract, 'id'> {}

export interface UpdateUserContract
	extends Omit<UserContract, 'id' | 'userId'> {
	contractId: number;
}

interface RequestHardSkillInfo {
	id: number;
	seniorityLevelId: string | null;
}

export type UserNotAssignedSpecializationsResponse =
	Array<UserNotAssignedSpecialization>;

export interface CreateSnapshotSpecializationParams {
	snapshotDate: string;
	userId: number;
	userSpecializations: Array<{
		specializationTemplateId: number;
		hardSkills: Array<RequestHardSkillInfo>;
	}>;
}

export interface AddSpecializationToUserParams {
	userId: number;
	specializationTemplateId: number;
	hardSkills: Array<RequestHardSkillInfo>;
	specializationSeniorityLevelId: string;
}

export interface UpdateUserHardSkills {
	userId: number;
	specializationTemplateId: number;
	hardSkills: Array<RequestHardSkillInfo>;
}

export interface UpdateSnapshotSpecializationParams
	extends Omit<CreateSnapshotSpecializationParams, 'snapshotDate'> {
	snapshotId: number;
}

export interface UpdateUserPrimarySpecialization {
	userId: number;
	newSpecializationTemplateId: number;
}

export interface UserDevelopmentPlanRequestProps {
	userId: number;
	startDate: string;
	deadlineDate: string | null;
	endDate?: string;
	summary: string;
	notes: string | null;
	conversationId?: number | undefined | null;
}

export interface UserDevelopmentPlanProps {
	id: number;
	startDate: string;
	deadlineDate: string | null;
	endDate?: string;
	summary: string;
	notes: string | null;
	conversationId?: number | undefined;
}

export interface CreateUserFeedback {
	externalFeedbackDate: string;
	externalFeedbackType: string;
	feedback: string;
	actionsTodo?: string;
	reviewer?: string;
	conversationId?: number;
	projectId: number;
	userId: number;
}

export type UpdateUserFeedback = Pick<
	CreateUserFeedback,
	'externalFeedbackType' | 'actionsTodo' | 'conversationId' | 'reviewer'
> & {
	id: number;
};
export type PerformanceReviewQueryFilter = Partial<
	PerformanceReviewFilter & { id: number }
>;

export interface GetPerformanceReviewQueryParams
	extends OdataParams<PerformanceReviewQueryFilter> {
	userId: number | undefined;
}

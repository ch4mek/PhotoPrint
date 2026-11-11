import { ByPageResponse } from '@/queries/types';

export type FinanceOrderingField = 'UserId' | 'UserFullName' | 'DepartmentId';

interface UserPersonalNameFields {
	firstName: string;
	lastName: string;
	firstNameRu: string | null;
	lastNameRu: string | null;
	patronymicRu: string | null;
}

export interface FinanceUser extends UserPersonalNameFields {
	id: number;
	departmentId: string | null;
	isActive: boolean;
}

export interface ProjectRate {
	projectRateId: number | null;
	projectId: number;
	projectName: string;
	projectColor: string | null;
	clientId: number;
	clientName: string;
	clientLegalEntity: string | null;
	hourlyRate: number | null;
	currencyId: string | null;
	canRateBeManagedByCurrentUser: boolean;
}

export interface RateUserItem {
	user: FinanceUser;
	projectRates: ProjectRate[];
	totalRate: number | null;
	currencyId: string | null;
	canUserInfoBeRetrievedByCurrentUser: boolean;
}

export interface GetProjectRatesInput {
	userId: string | null;
	userFullName: string | null;
	projectName: string | null;
	clientName: string | null;
	clientLegalEntity: string | null;
	departmentIds: string[] | null;
	orderingField: FinanceOrderingField;
	isDescendingOrdering: boolean;
}

export type GetRatesResponse = ByPageResponse<RateUserItem>;

export interface ProjectRateInput {
	hourlyRate: number;
	currencyId: string;
	rateFrom: string;
}

interface ProjectRateRecord extends ProjectRateInput {
	id: number;
}

interface RateHistoryProjectRate extends ProjectRateRecord {
	canRateBeManagedByCurrentUser: boolean;
}

export interface CreateProjectRateParams extends ProjectRateInput {
	userId: number;
	projectId: number;
}

export interface UpdateProjectRateParams extends ProjectRateRecord {}

interface RateHistoryUser extends UserPersonalNameFields {
	id: number;
	departmentId: string | null;
	isActive: boolean;
}

interface ProjectSummary {
	id: number;
	projectName: string;
	clientId: number;
	color: string | null;
}

interface ProjectRateEntityProject extends ProjectSummary {
	clientName: string;
	isActive: boolean;
}

interface ProjectRateEntityUser extends FinanceUser {
	email: string;
	isActive: boolean;
}

export interface ProjectRateEntity extends ProjectRateRecord {
	userId: number;
	projectId: number;
	user: ProjectRateEntityUser;
	project: ProjectRateEntityProject;
}

export interface GetRateHistoryResponse {
	userId: number;
	user: RateHistoryUser;
	projectId: number;
	project: ProjectSummary;
	canRateBeAddedByCurrentUser: boolean;
	projectRates: RateHistoryProjectRate[];
}

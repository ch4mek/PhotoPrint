import { Dispatch, SetStateAction } from 'react';

import { TagTypes } from '@/components/DateTag/types';
import { GridCellDefaultProps, FromToProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { DynamicHeightCellDefaultProps } from '@/components/Grid/renderers/types';
import { Roles } from '@/constants/roles';
import { DevelopmentSectionProps } from '@/pages/TeamBoardUser/components/TeamBoardUserSummaryTab/DevelopmentPlanSection/types';
import { DevelopmentPlanRow } from '@/types/developmentPlan';
import { ConversationDocument } from '@/types/document';
import { FullHardSkillInfo } from '@/types/hardSkills';
import { SetFilters } from '@/types/helpers';
import { PersonalDataResponse } from '@/types/personalData';
import { UserSpecialization } from '@/types/teamboard';
import { UserProjectLookup } from '@/types/userProjectLookup';

import { SpecializationTableFilters } from './components/SpecializationTable/types';

export interface TeamboardUserPageProps {
	handleClose: () => void;
	id: number | string | undefined;
	setPersonalDataChanged?: VoidFunction;
}

export interface HardSkillRow extends GridCellDefaultProps {
	id: number;
	isPrimary: boolean;
	seniority: string;
	name: string;
}

export type ProjectRow = GridCellDefaultProps &
	UserProjectLookup & {
		hideBottomBorder?: boolean;
	};

interface HardSkillFilters {
	name: string;
	seniority: string[];
}

export type HardSkillTableFilters = HardSkillFilters;

export enum SpecializationTableFiltersKeys {
	specializationName = 'name',
	set = 'set',
	reset = 'reset',
}

export interface HardSkillsFilterSetter {
	name: (name: string) => void;
	seniority: (seniorities: string[]) => void;
	set: (allFilters: HardSkillTableFilters) => void;
}

export type SpecializationAction = {
	type: SpecializationTableFiltersKeys;
	payload: string | SpecializationTableFilters;
};

export interface PersonalDataContextValues {
	isFetching: boolean;
	isLoading: boolean;
	showLoader: boolean;
	refetch: () => void;
	userId: number | undefined;
	position: Roles;
	data: PersonalDataResponse;
	setPersonalDataChanged?: VoidFunction;
	accessLevel: TeamboardUserAccessLevel;
	profileKind: TeamboardUserProfileKind;
}

export interface TabContextValues {
	handleTabChange: (newTab: SetStateAction<number>) => void;
}

interface SkillTableFilterOptions {
	seniorityLevels: Array<SelectionModalOption>;
}

export type HardSkillTableFilterOptions = SkillTableFilterOptions;

export interface ConversationRow extends DynamicHeightCellDefaultProps {
	id: number;
	date: string;
	conversationResult?: FeedbackType;
	conversationType: string;
	theses: string;
	employeeTheses: string;
	agreements: string;
	supervisor: string;
	height?: number;
	files: ConversationDocument[];
	links: string[];
	handleViewClick?: (id?: number | string) => () => void;
}

export enum ConversationTableFiltersKeys {
	date = 'date',
	conversationResult = 'conversationResult',
	conversationType = 'conversationType',
	theses = 'theses',
	agreements = 'agreements',
	employeeTheses = 'employeeTheses',
	supervisor = 'supervisor',
	set = 'set',
}

export interface ConversationAction {
	type: ConversationTableFiltersKeys;
	payload?: FromToProps<Date> | string | ConversationFilter | string[];
}

export interface ConversationFilterSetter {
	date: (date: FromToProps<Date>) => void;
	conversationResult: (conversationResult: string[]) => void;
	conversationType: (conversationType: string[]) => void;
	theses: (theses: string) => void;
	agreements: (agreements: string) => void;
	employeeTheses: (employeeTheses: string) => void;
	supervisor: (supervisor: string) => void;
	set: (filter: ConversationFilter) => void;
}

export interface ConversationFilter {
	date: FromToProps<Date>;
	conversationResult: string[];
	conversationType: string[];
	theses: string;
	agreements: string;
	employeeTheses: string;
	supervisor: string;
}

export enum FeedbackType {
	Great = 'Great',
	Normal = 'Normal',
	Bad = 'Bad',
}

export interface FeedbackRow extends DynamicHeightCellDefaultProps {
	id: number;
	date: string;
	type: FeedbackType;
	client: string;
	project: string;
	feedback: string;
	actions: string;
	reviewer: string;
	conversationId: number | null;
}

export enum FeedbackTableFiltersKeys {
	date = 'date',
	type = 'type',
	client = 'client',
	project = 'project',
	feedback = 'feedback',
	actions = 'actions',
	reviewer = 'reviewer',
	set = 'set',
}

export interface FeedbackAction {
	type: FeedbackTableFiltersKeys;
	payload?: FromToProps<Date> | string | string[] | FeedbackFilter;
}

export type FeedbackFilterSetter = SetFilters<FeedbackFilter>;

export interface FeedbackFilter {
	date: FromToProps<Date>;
	type: string[];
	client: string;
	project: string;
	feedback: string;
	actions: string;
	reviewer: string;
}

export type SpecializationSnapshotsByDates = Record<
	string,
	UserSpecialization | undefined
>;

export enum SpecializationRowType {
	Specialization = 'specialization',
	HardSkills = 'hardSkills',
}

export interface SpecializationRow extends GridCellDefaultProps {
	id: number;
	specializationId: number;
	specializationTemplateId: number;
	hardSkills: Array<FullHardSkillInfo>;
	isPrimary: boolean;
	name: string;
	snapshots: SpecializationSnapshotsByDates;
	isExpanded: boolean;
	height: number;
	type: SpecializationRowType;
	isEven: boolean;
}

export interface SpecializationHardSkillRow extends GridCellDefaultProps {
	id: number;
	specializationId: number;
	name: string;
	snapshots: SpecializationSnapshotsByDates;
	isActive: boolean;
}
export enum PerformanceReviewTableFiltersKeys {
	performanceReviewDate = 'performanceReviewDate',
	theses = 'theses',
	agreements = 'agreements',
	employeeTheses = 'employeeTheses',
	set = 'set',
}

export interface PerformanceReviewFilter {
	performanceReviewDate: FromToProps<Date>;
	theses: string;
	agreements: string;
	employeeTheses: string;
}

export interface PerformanceReviewFilterSetter {
	performanceReviewDate: (date: FromToProps<Date>) => void;
	theses: (theses: string) => void;
	agreements: (agreements: string) => void;
	employeeTheses: (employeeTheses: string) => void;
	set: (filter: PerformanceReviewFilter) => void;
}

export interface PerformanceReviewRow extends DynamicHeightCellDefaultProps {
	id: number;
	performanceReviewDate: string;
	performanceReviewDateType: TagTypes;
	conversationId: number;
	theses: string;
	employeeTheses: string;
	agreements: string;
	files: ConversationDocument[];
	links: string[];
}

export enum TeamBoardTabsId {
	SUMMARY,
	SALARY_DETAILS,
	PROJECTS,
	HARD_SKILLS,
	SOFT_SKILLS,
	LIST_OF_INTERVIEWS,
	CONVERSATIONS,
	INTERVIEWS,
	EXTERNAL_FEEDBACK,
	DEVELOPMENT_PLAN,
	PERFORMANCE_REVIEW,
	ABSENCES,
	CONTRACTS,
	EDUCATION,
	EQUIPMENT,
}

export interface TabsSectionProps {
	minHeight: number | null;
	maxHeight: number | null;
}

export enum DevelopmentPlanMutationMode {
	None,
	AddOrEdit,
	Approve,
	Delete,
}

export interface UseDevelopmentPlanFormStateProps
	extends DevelopmentSectionProps {
	refetch: VoidFunction;
	setRows?: Dispatch<SetStateAction<DevelopmentPlanRow[]>>;
	rows?: DevelopmentPlanRow[];
}

export interface UseTabsSectionHeightParams {
	activeTab: number;
	departmentId: string | null;
	accessLevel: TeamboardUserAccessLevel;
	position?: Roles;
}

export const enum TeamboardUserAccessLevel {
	Full = 'full',
	LimitedHR = 'limited-hr',
	LimitedCoordinator = 'limited-coordinator',
	LimitedAccountant = 'limited-accountant',
	None = 'none',
}

export const enum TeamboardUserProfileKind {
	Employee = 'employee',
	Intern = 'intern',
}

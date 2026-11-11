import { spaces } from '@mms/mms-ui-library/theme';

import {
	GRID_ROW_HEIGHT,
	HEADER_PAGINATION_HEIGHT,
} from '@/components/Grid/constants';
import { UserClaims } from '@/types/claims';

import { AbsencesTab } from './components/AbsencesTab';
import { ContractsTab } from './components/ContractsTab';
import { EducationTab } from './components/EducationTab';
import { EquipmentTab } from './components/EquipmentTab';
import { HardSkillsTab } from './components/HardSkillsTab';
import { JiraSettingsTab } from './components/JiraSettingsTab';
import { PerformanceReviewTab } from './components/PerformanceReviewTab';
import { PlanTab } from './components/PlanTab';
import { ProjectsTab } from './components/ProjectsTab';
import { SalaryTab } from './components/SalaryTab';
import { SignatureTab } from './components/SignatureTab';
import { SoftSkillsTab } from './components/SoftSkillsTab';
import { SummaryTab } from './components/SummaryTab';
import { UserProfileTabConfig, UserProfileTabsId } from './types';

const {
	SUMMARY,
	ABSENCES,
	EQUIPMENT,
	EDUCATION,
	SALARY,
	PROJECTS,
	HARD_SKILLS,
	SOFT_SKILLS,
	DEVELOPMENT_PLAN,
	PERFORMANCE_REVIEW,
	CONTRACTS,
	SIGNATURE,
	JIRA_SETTINGS,
} = UserProfileTabsId;

export const tabs = [
	{
		id: SUMMARY,
		Component: SummaryTab,
	},
	{
		id: ABSENCES,
		Component: AbsencesTab,
		requiredClaims: [UserClaims.ViewAbsences],
	},
	{
		id: EQUIPMENT,
		Component: EquipmentTab,
	},
	{
		id: EDUCATION,
		Component: EducationTab,
	},
	{
		id: SALARY,
		Component: SalaryTab,
	},
	{
		id: PROJECTS,
		Component: ProjectsTab,
	},
	{
		id: HARD_SKILLS,
		Component: HardSkillsTab,
	},
	{
		id: SOFT_SKILLS,
		Component: SoftSkillsTab,
	},
	{
		id: DEVELOPMENT_PLAN,
		Component: PlanTab,
	},
	{
		id: PERFORMANCE_REVIEW,
		Component: PerformanceReviewTab,
	},
	{
		id: CONTRACTS,
		Component: ContractsTab,
	},
	{
		id: SIGNATURE,
		Component: SignatureTab,
	},
	{
		id: JIRA_SETTINGS,
		Component: JiraSettingsTab,
	},
];

export const userProfileTabs: UserProfileTabConfig[] = [
	{
		id: SUMMARY,
		text: 'summary',
	},
	{
		id: ABSENCES,
		text: 'absences',
		requiredClaims: [UserClaims.ViewAbsences],
	},
	{
		id: EQUIPMENT,
		text: 'equipment',
	},
	{
		id: EDUCATION,
		text: 'education',
	},
	{
		id: SALARY,
		text: 'salary',
	},
	{ id: PROJECTS, text: 'projects' },
	{
		id: HARD_SKILLS,
		text: 'hardSkills',
	},
	{
		id: SOFT_SKILLS,
		text: 'softSkills',
	},
	{
		id: DEVELOPMENT_PLAN,
		text: 'developmentPlan',
	},
	{
		id: PERFORMANCE_REVIEW,
		text: 'performanceReview',
	},
	{
		id: CONTRACTS,
		text: 'contracts',
	},
	{
		id: SIGNATURE,
		text: 'signature',
	},
	{
		id: JIRA_SETTINGS,
		text: 'jiraSettings',
	},
];

const DEFAULT_MAX_ROW_NUMBER = 5;
export const MAX_SECTION_ROWS_COUNT = 3;
export const CERTIFICATE_ROW_HEIGHT = 108;
export const GAP_BETWEEN_GRIDS = 10;

export const MIN_EDUCATION_GRID_HEIGHT =
	DEFAULT_MAX_ROW_NUMBER * GRID_ROW_HEIGHT + HEADER_PAGINATION_HEIGHT;

export const MIN_CERTIFICATE_GRID_HEIGHT =
	CERTIFICATE_ROW_HEIGHT * MAX_SECTION_ROWS_COUNT + HEADER_PAGINATION_HEIGHT;

const ADD_BUTTON_HEIGHT = 36;

export const MIN_JIRA_SETTINGS_GRID_HEIGHT =
	DEFAULT_MAX_ROW_NUMBER * GRID_ROW_HEIGHT +
	HEADER_PAGINATION_HEIGHT +
	ADD_BUTTON_HEIGHT +
	spaces.s;

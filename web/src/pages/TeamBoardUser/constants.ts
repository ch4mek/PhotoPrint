import { css, Theme } from '@emotion/react';
import { spaces } from '@mms/mms-ui-library/theme';

import {
	GRID_ROW_HEIGHT,
	HEADER_PAGINATION_HEIGHT,
} from '@/components/Grid/constants';
import { UserClaims } from '@/types/claims';

import { AbsencesTab } from './components/AbsencesTab';
import { ContractsTab } from './components/ContractsTab';
import { ConversationsTab } from './components/ConversationsTab';
import { DevelopmentPlanTab } from './components/DevelopmentPlanTab';
import { EducationTab } from './components/EducationTab';
import { EquipmentTab } from './components/EquipmentTab';
import { FeedbackTab } from './components/FeedbackTab';
import { HardSkillsTab } from './components/HardSkillsTab';
import { InterviewsTab } from './components/InterviewsTab';
import { PerformanceReviewTab } from './components/PerformanceReviewTab';
import { SalaryTab } from './components/SalaryTab';
import { SoftSkillsTab } from './components/SoftSkillsTab';
import { TeamboardProjectAccessTab } from './components/TeamboardProjectAccessTab';
import { TeamBoardUserSummaryTab } from './components/TeamBoardUserSummaryTab';
import { TeamBoardTabsId, TeamboardUserAccessLevel } from './types';

export const DEFAULT_LOADER_ROWS_COUNT = 1;
export const SECTION_TEXT_MAX_ROW_COUNT = 5;

const {
	SUMMARY,
	SALARY_DETAILS,
	PROJECTS,
	HARD_SKILLS,
	SOFT_SKILLS,
	CONVERSATIONS,
	INTERVIEWS,
	EXTERNAL_FEEDBACK,
	DEVELOPMENT_PLAN,
	PERFORMANCE_REVIEW,
	ABSENCES,
	CONTRACTS,
	EDUCATION,
	EQUIPMENT,
} = TeamBoardTabsId;

export const teamBoardUserTabs = [
	{
		id: SUMMARY,
		text: 'Summary',
		accessLevels: [
			TeamboardUserAccessLevel.Full,
			TeamboardUserAccessLevel.LimitedHR,
			TeamboardUserAccessLevel.LimitedCoordinator,
			TeamboardUserAccessLevel.LimitedAccountant,
		],
	},
	{
		id: PROJECTS,
		text: 'Projects',
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: HARD_SKILLS,
		text: 'Hard skills',
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: SOFT_SKILLS,
		text: 'Soft skills',
		accessLevels: [TeamboardUserAccessLevel.Full],
	},

	{
		id: CONVERSATIONS,
		text: 'Conversations',
		accessLevels: [
			TeamboardUserAccessLevel.Full,
			TeamboardUserAccessLevel.LimitedHR,
		],
	},
	{
		id: INTERVIEWS,
		text: 'Interviews',
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: EXTERNAL_FEEDBACK,
		text: 'Ext. feedback',
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: DEVELOPMENT_PLAN,
		text: 'Dev. plan',
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: PERFORMANCE_REVIEW,
		text: 'Perf. review',
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: ABSENCES,
		text: 'Absences',
		accessLevels: [
			TeamboardUserAccessLevel.Full,
			TeamboardUserAccessLevel.LimitedHR,
			TeamboardUserAccessLevel.LimitedCoordinator,
			TeamboardUserAccessLevel.LimitedAccountant,
		],
		requiredClaims: [UserClaims.ViewAbsences],
	},
	{
		id: CONTRACTS,
		text: 'Contracts',
		accessLevels: [
			TeamboardUserAccessLevel.Full,
			TeamboardUserAccessLevel.LimitedHR,
			TeamboardUserAccessLevel.LimitedAccountant,
		],
	},
	{
		id: SALARY_DETAILS,
		text: 'Salary',
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: EDUCATION,
		text: 'Education',
		accessLevels: [
			TeamboardUserAccessLevel.Full,
			TeamboardUserAccessLevel.LimitedHR,
		],
	},
	{
		id: EQUIPMENT,
		text: 'Equipment',
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
];

export const alternateTabTableRowsColor = (theme: Theme) => css`
	.rdg-row-odd {
		background-color: ${theme.palette['even-stroke-base-1']};
	}
`;

export const tabs = [
	{
		id: SUMMARY,
		Component: TeamBoardUserSummaryTab,
		accessLevels: [
			TeamboardUserAccessLevel.Full,
			TeamboardUserAccessLevel.LimitedHR,
			TeamboardUserAccessLevel.LimitedCoordinator,
			TeamboardUserAccessLevel.LimitedAccountant,
		],
	},
	{
		id: SALARY_DETAILS,
		Component: SalaryTab,
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: PROJECTS,
		Component: TeamboardProjectAccessTab,
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: HARD_SKILLS,
		Component: HardSkillsTab,
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: CONVERSATIONS,
		Component: ConversationsTab,
		accessLevels: [
			TeamboardUserAccessLevel.Full,
			TeamboardUserAccessLevel.LimitedHR,
		],
	},
	{
		id: INTERVIEWS,
		Component: InterviewsTab,
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: EXTERNAL_FEEDBACK,
		Component: FeedbackTab,
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: SOFT_SKILLS,
		Component: SoftSkillsTab,
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: DEVELOPMENT_PLAN,
		Component: DevelopmentPlanTab,
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: PERFORMANCE_REVIEW,
		Component: PerformanceReviewTab,
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
	{
		id: ABSENCES,
		Component: AbsencesTab,
		accessLevels: [
			TeamboardUserAccessLevel.Full,
			TeamboardUserAccessLevel.LimitedHR,
			TeamboardUserAccessLevel.LimitedCoordinator,
			TeamboardUserAccessLevel.LimitedAccountant,
		],
		requiredClaims: [UserClaims.ViewAbsences],
	},
	{
		id: CONTRACTS,
		Component: ContractsTab,
		accessLevels: [
			TeamboardUserAccessLevel.Full,
			TeamboardUserAccessLevel.LimitedHR,
			TeamboardUserAccessLevel.LimitedAccountant,
		],
	},
	{
		id: EDUCATION,
		Component: EducationTab,
		accessLevels: [
			TeamboardUserAccessLevel.Full,
			TeamboardUserAccessLevel.LimitedHR,
		],
	},
	{
		id: EQUIPMENT,
		Component: EquipmentTab,
		accessLevels: [TeamboardUserAccessLevel.Full],
	},
];

export const POPUP_TITLE = 'Personal info';

export const UPCOMING_PERFOMANCE_REVIEW_HEIGHT = 28;
export const COMMENT_SECTION_HEIGHT = 124;
export const GAPS_COUNT = 2;

const DEFAULT_MAX_ROW_NUMBER = 5;
export const MAX_SECTION_ROWS_COUNT = 3;
export const CERTIFICATE_ROW_HEIGHT = 108;
const BUTTON_HEIGHT = 36;

export const MIN_EDUCATION_GRID_HEIGHT =
	DEFAULT_MAX_ROW_NUMBER * GRID_ROW_HEIGHT +
	HEADER_PAGINATION_HEIGHT +
	BUTTON_HEIGHT +
	spaces.s;

export const getMinCertificateGridHeight = (hasAddButton = true) =>
	CERTIFICATE_ROW_HEIGHT * MAX_SECTION_ROWS_COUNT +
	HEADER_PAGINATION_HEIGHT +
	(hasAddButton ? BUTTON_HEIGHT + spaces.s : 0);

export const MIN_SALARY_GRID_HEIGHT =
	DEFAULT_MAX_ROW_NUMBER * GRID_ROW_HEIGHT +
	HEADER_PAGINATION_HEIGHT +
	BUTTON_HEIGHT +
	spaces.s;

export const MIN_PAYSLIPS_GRID_HEIGHT =
	DEFAULT_MAX_ROW_NUMBER * GRID_ROW_HEIGHT + HEADER_PAGINATION_HEIGHT;

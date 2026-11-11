import { Resources } from 'i18next';
import { SetStateAction } from 'react';

import { UserClaims } from '@/types/claims';
import { PersonalDataResponse } from '@/types/personalData';
import { ProjectsTableFilters } from '@/types/projectsTableFilters';
import { UserProjectLookup } from '@/types/userProjectLookup';

export interface TabContextValues {
	handleTabChange: (newTab: SetStateAction<number>) => void;
}

export interface PersonalDataContextValues {
	isFetching: boolean;
	isLoading: boolean;
	showLoader: boolean;
	refetch: () => void;
	userId: number | undefined;
	data: PersonalDataResponse;
}

export enum UserProfileTabsId {
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
}

export interface UserProfileProps {
	handleClose: () => void;
}

export type ProjectTableRow = UserProjectLookup & {
	disabled: boolean;
};

export type ProjectTableColumnWidths = Record<
	keyof ProjectsTableFilters,
	number
>;

type UserProfileTabLabelKey = keyof Resources['profile']['tabs'];

export type UserProfileTabConfig = {
	id: UserProfileTabsId;
	text: UserProfileTabLabelKey;
	requiredClaims?: UserClaims[];
};

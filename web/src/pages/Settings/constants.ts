import { lazy } from 'react';

import { UserClaims } from '@/types/claims';

const ProjectPermissionsTab = lazy(async () => ({
	default: (await import('./components/ProjectPermissionsTab'))
		.ProjectPermissionsTab,
}));
const SkillsTab = lazy(async () => ({
	default: (await import('./components/SkillsTab')).SkillsTab,
}));
const CalendarTab = lazy(async () => ({
	default: (await import('./components/CalendarTab')).CalendarTab,
}));
const PortalSettingsTab = lazy(async () => ({
	default: (await import('./components/PortalSettingsTab')).PortalSettingsTab,
}));
const DepartmentClaimsTab = lazy(async () => ({
	default: (await import('./components/ClaimsTabs')).DepartmentClaimsTab,
}));
const RoleClaimsTab = lazy(async () => ({
	default: (await import('./components/ClaimsTabs')).RoleClaimsTab,
}));
const UserClaimsTab = lazy(async () => ({
	default: (await import('./components/ClaimsTabs')).UserClaimsTab,
}));
const AbsencesTab = lazy(async () => ({
	default: (await import('./components/AbsencesTab')).AbsencesTab,
}));
const KPITab = lazy(async () => ({
	default: (await import('./components/KPITab')).KPITab,
}));

export const ABSENCES_TAB_ID = 8;

export const settingsAbsencesTab = [
	{
		id: ABSENCES_TAB_ID,
		text: 'Absences',
		Component: AbsencesTab,
		claims: [UserClaims.ManageAbsenceSettings],
	},
];

export const commonSettingsPageTabs = [
	{
		id: 1,
		text: 'Project permissions',
		Component: ProjectPermissionsTab,
		claims: [],
	},
	{ id: 2, text: 'Skills', Component: SkillsTab, claims: [] },
	{ id: 3, text: 'Calendar', Component: CalendarTab, claims: [] },
	...settingsAbsencesTab,
];

const chiefsSettingsPageTabs = [
	{ id: 4, text: 'Portal settings', Component: PortalSettingsTab, claims: [] },
	{
		id: 5,
		text: 'Department claims',
		Component: DepartmentClaimsTab,
		claims: [],
	},
	{ id: 6, text: 'Role claims', Component: RoleClaimsTab, claims: [] },
	{ id: 7, text: 'User claims', Component: UserClaimsTab, claims: [] },
	{ id: 9, text: 'KPI', Component: KPITab, claims: [] },
];

export const settingsPageTabs = [
	...commonSettingsPageTabs,
	...chiefsSettingsPageTabs,
];

export const paddingsExcludedTabIds = [ABSENCES_TAB_ID];

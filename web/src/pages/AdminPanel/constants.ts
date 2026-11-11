import { lazy } from 'react';

import { TabWithComponent } from '@/types/common';

const UsersTab = lazy(async () => ({
	default: (await import('./components/UsersTab')).UsersTab,
}));
const ProjectsTab = lazy(async () => ({
	default: (await import('./components/ProjectsTab')).ProjectsTab,
}));
const ClientsTab = lazy(async () => ({
	default: (await import('./components/ClientsTab')).ClientsTab,
}));
const DepartmentTab = lazy(async () => ({
	default: (await import('./components/DepartmentsTab')).DepartmentsTab,
}));

const commonAdminPanelPageTabs: TabWithComponent[] = [
	{ id: 3, text: 'Clients', Component: ClientsTab },
	{ id: 2, text: 'Projects', Component: ProjectsTab },
];

export const systemAdministratorAdminPanelPageTabs: TabWithComponent[] = [
	{ id: 1, text: 'Users', Component: UsersTab },
	{
		id: 4,
		text: 'Departments',
		Component: DepartmentTab,
	},
];

export const HRsAdminPanelPageTabs: TabWithComponent[] = [
	{ id: 1, text: 'Users', Component: UsersTab },
];

export const adminPanelPageTabs: TabWithComponent[] = [
	...commonAdminPanelPageTabs,
	...systemAdministratorAdminPanelPageTabs,
];

export const ADMIN_PANEL_TAB_INDEX_LOCAL_STORAGE_KEY = 'admin-panel-tab';

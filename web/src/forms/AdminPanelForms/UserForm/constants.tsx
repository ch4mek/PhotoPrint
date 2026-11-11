import { PersonTabIcon } from '@mms/mms-ui-library/icons/base/PersonTabIcon';
import { ProjectTabIcon } from '@mms/mms-ui-library/icons/base/ProjectTabIcon';

import { Roles } from '@/constants/roles';

export enum UserFormTabs {
	USER,
	GROUP_MEMBERS,
	PROJECT_ACCESS,
}

export const USER_FORM_TABS_HEADER = [
	{
		id: UserFormTabs.USER,
		icon: <PersonTabIcon />,
		text: 'User',
	},
	{
		id: UserFormTabs.GROUP_MEMBERS,
		icon: null,
		text: 'Group members',
	},
	{
		id: UserFormTabs.PROJECT_ACCESS,
		icon: <ProjectTabIcon />,
		text: 'Project access',
	},
];

export const DEFAULT_USER_TAB_HEIGHT = 469;

export const entityName = 'User';

export const formName = 'userForm';

export const USER_FORM_POPUP_HEIGHT_TABLET = 627;

const { CTO, CEO } = Roles;

export const roleRestrictions: Record<string, string[]> = {
	[CEO]: [CEO],
	[CTO]: [CEO, CTO],
};

export const MAIN_COUNTRY_NAME = 'Belarus';

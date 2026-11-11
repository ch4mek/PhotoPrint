import { ConfluenceIcon } from '@mms/mms-ui-library/icons/logo/ConfluenceIcon';
import { DiscountsIcon } from '@mms/mms-ui-library/icons/logo/DiscountsIcon';
import { GitlabIcon } from '@mms/mms-ui-library/icons/logo/GitlabIcon';
import { HangfireIcon } from '@mms/mms-ui-library/icons/logo/HangfireIcon';
import { JiraIcon } from '@mms/mms-ui-library/icons/logo/JiraIcon';
import { LearnIcon } from '@mms/mms-ui-library/icons/logo/LearnIcon';
import { ModsenAppIcon } from '@mms/mms-ui-library/icons/logo/ModsenAppIcon';
import { NetworkIcon } from '@mms/mms-ui-library/icons/logo/NetworkIcon';
import { RecruitmentIcon } from '@mms/mms-ui-library/icons/logo/RecruitmentIcon';
import { ServiceDeskIcon } from '@mms/mms-ui-library/icons/logo/ServiceDeskIcon';
import { WikiIcon } from '@mms/mms-ui-library/icons/logo/WikiIcon';

import { API_URL } from '@/constants/queryPaths';
import { AllRoles, Roles } from '@/constants/roles';
import { Routes } from '@/constants/routes';
import { UserClaims } from '@/types/claims';

export const SIDEBAR_MENU_WIDTH = 420;
export const MOBILE_SIDEBAR_MENU_WIDTH = 356;
export const SIDEBAR_POPOVER_OFFSET: [number, number] = [0, 19];
export const HANGFIRE_APP_LINK_TITLE = 'Hangfire';

export const headerMenuLinks = [
	{
		title: 'Jira',
		href: 'https://jira.modsen.app/secure/Dashboard.jspa',
		icon: <JiraIcon />,
		roles: AllRoles,
		claims: [],
		hasSpecificColor: true,
		shouldReplace: false,
	},
	{
		title: 'Confluence',
		href: 'https://confluence.modsen.app',
		icon: <ConfluenceIcon />,
		roles: AllRoles,
		claims: [],
		hasSpecificColor: true,
		shouldReplace: false,
	},
	{
		title: 'Service desk',
		href: 'https://jira.modsen.app/servicedesk',
		icon: <ServiceDeskIcon />,
		roles: AllRoles,
		claims: [],
		hasSpecificColor: true,
		shouldReplace: false,
	},
	{
		title: 'Learn',
		href: 'https://learn.modsen.app',
		icon: <LearnIcon />,
		roles: AllRoles,
		claims: [],
		hasSpecificColor: true,
		shouldReplace: false,
	},
	{
		title: 'GitLab',
		href: 'https://gitlab.modsen.app',
		icon: <GitlabIcon />,
		roles: AllRoles,
		claims: [],
		hasSpecificColor: true,
		shouldReplace: false,
	},
	{
		title: 'Wiki',
		href: Routes.wiki,
		icon: <WikiIcon />,
		roles: [],
		claims: [UserClaims.ViewWiki],
		hasSpecificColor: true,
		shouldReplace: true,
	},
	{
		title: 'Discounts',
		href: Routes.discounts,
		icon: <DiscountsIcon />,
		roles: [],
		claims: [UserClaims.ViewDiscounts],
		hasSpecificColor: true,
		shouldReplace: true,
	},
	{
		title: 'Recruitment',
		href: Routes.recruitment,
		icon: <RecruitmentIcon />,
		roles: [Roles.CTO],
		claims: [UserClaims.AccessToRecruitment],
		hasSpecificColor: true,
		shouldReplace: true,
	},
	{
		title: 'Network',
		href: 'https://network.modsen.app/login',
		icon: <NetworkIcon />,
		roles: AllRoles,
		claims: [],
		hasSpecificColor: true,
		shouldReplace: false,
	},
	{
		title: HANGFIRE_APP_LINK_TITLE,
		href: `${API_URL}/hangfire-dashboard?access_token=`,
		icon: <HangfireIcon />,
		roles: [],
		claims: [UserClaims.HangfireDashboardAccess],
		hasSpecificColor: true,
		shouldReplace: false,
	},
];

export const appModsenLink = {
	title: 'Modsen app',
	href: Routes.home,
	icon: <ModsenAppIcon />,
	roles: [Roles.CTO],
	claims: [UserClaims.ViewWiki, UserClaims.ViewDiscounts],
	hasSpecificColor: true,
	shouldReplace: false,
};

export const sidebarMenuSections = {
	other: 'Other apps',
	modules: 'Modules',
	chatWidget: 'Digital HR',
};

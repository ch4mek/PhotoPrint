import { ClientsIcon, PnLIcon } from '@mms/mms-ui-library/icons/base';
import { AdminPanelIcon } from '@mms/mms-ui-library/icons/base/AdminPanelIcon';
import { AttendanceIcon } from '@mms/mms-ui-library/icons/base/AttendanceIcon';
import { BookingIcon } from '@mms/mms-ui-library/icons/base/BookingIcon';
import { DashboardIcon } from '@mms/mms-ui-library/icons/base/DashboardIcon';
import { EquipmentIcon } from '@mms/mms-ui-library/icons/base/EquipmentIcon';
import { HomeIcon } from '@mms/mms-ui-library/icons/base/HomeIcon';
import { MerchIcon } from '@mms/mms-ui-library/icons/base/MerchIcon';
import { PayloadIcon } from '@mms/mms-ui-library/icons/base/PayloadIcon';
import { PerformanceReviewIcon } from '@mms/mms-ui-library/icons/base/PerformanceReviewIcon';
import { ReportsIcon } from '@mms/mms-ui-library/icons/base/ReportsIcon';
import { RequestsIcon } from '@mms/mms-ui-library/icons/base/RequestsIcon';
import { SettingsIcon } from '@mms/mms-ui-library/icons/base/SettingsIcon';
import { TeamBoardIcon } from '@mms/mms-ui-library/icons/base/TeamBoardIcon';
import { TimeSheetIcon } from '@mms/mms-ui-library/icons/base/TimeSheetIcon';
import { CoursesIcon } from '@mms/mms-ui-library/icons/education/CoursesIcon';

import {
	AllRoles,
	allRolesExceptIntern,
	managerRolesWithGroupManager,
	managerRolesWithGroupManagerAndSA,
} from '@/constants/roles';
import { Routes } from '@/constants/routes';
import { UserClaims } from '@/types/claims';

import { NavigationItem } from './types';

const firstSectionTabs: NavigationItem[] = [
	{
		id: 1,
		label: 'Home',
		icon: <HomeIcon />,
		payload: {
			slug: Routes.home,
			roles: AllRoles,
			claims: [UserClaims.ViewHome],
		},
	},
	{
		id: 2,
		label: 'Dashboard',
		icon: <DashboardIcon />,
		payload: {
			slug: Routes.dashboard,
			roles: AllRoles,
			claims: [UserClaims.ViewDashboard],
		},
	},
	{
		id: 3,
		label: 'Timesheet',
		icon: <TimeSheetIcon />,
		payload: {
			slug: Routes.timeSheet,
			roles: AllRoles,
			claims: [],
		},
	},
	{
		id: 4,
		label: 'Reports',
		icon: <ReportsIcon />,
		payload: {
			slug: Routes.reports,
			roles: allRolesExceptIntern,
			claims: [],
		},
	},
	{
		id: 5,
		label: 'Booking',
		icon: <BookingIcon />,
		payload: {
			slug: Routes.booking,
			roles: allRolesExceptIntern,
			claims: [],
		},
	},
];

const secondSectionTabs: NavigationItem[] = [
	{
		id: 6,
		label: 'Teamboard',
		icon: <TeamBoardIcon />,
		payload: {
			slug: Routes.teamBoard,
			roles: managerRolesWithGroupManager,
			claims: [],
		},
	},
	{
		id: 7,
		label: 'Payload',
		icon: <PayloadIcon />,
		payload: {
			slug: Routes.payload,
			roles: managerRolesWithGroupManager,
			claims: [],
		},
	},
	{
		id: 8,
		label: 'P&L',
		icon: <PnLIcon />,
		payload: {
			slug: Routes.profitAndLoss,
			roles: AllRoles,
			claims: [
				UserClaims.ViewRates,
				UserClaims.ViewAllRates,
				UserClaims.ViewManagementTab,
				UserClaims.ManageExpensesTab,
			],
		},
	},
	{
		id: 9,
		label: 'Staffing',
		icon: <ClientsIcon />,
		payload: {
			slug: Routes.staffing,
			roles: AllRoles,
			claims: [
				UserClaims.ManageInterviews,
				UserClaims.ManageAllInterviews,
				UserClaims.ViewProjectExits,
				UserClaims.ManageAllProjectExits,
			],
		},
	},
	{
		id: 10,
		label: 'Requests',
		icon: <RequestsIcon />,
		payload: {
			slug: Routes.requests,
			roles: AllRoles,
			claims: [
				UserClaims.ManageAllRequests,
				UserClaims.ManageRequests,
				UserClaims.ViewAllRequests,
			],
		},
	},
	{
		id: 11,
		label: 'Perf. review',
		icon: <PerformanceReviewIcon />,
		payload: {
			slug: Routes.performanceReview,
			roles: managerRolesWithGroupManager,
			claims: [],
		},
	},
	{
		id: 12,
		label: 'Attendance',
		icon: <AttendanceIcon />,
		payload: {
			slug: Routes.attendance,
			roles: managerRolesWithGroupManager,
			claims: [],
		},
	},
];

const thirdSectionTabs: NavigationItem[] = [
	{
		id: 13,
		label: 'Equipment',
		icon: <EquipmentIcon />,
		payload: {
			slug: Routes.equipment,
			roles: managerRolesWithGroupManagerAndSA,
			claims: [UserClaims.ManageAllEquipment],
		},
	},
	{
		id: 14,
		label: 'Merch',
		icon: <MerchIcon />,
		payload: {
			slug: Routes.merch,
			roles: AllRoles,
			claims: [UserClaims.ViewMerch],
		},
	},
];

const fourthSectionTabs: NavigationItem[] = [
	{
		id: 15,
		label: 'Admin Panel',
		icon: <AdminPanelIcon />,
		payload: {
			slug: Routes.adminPanel,
			roles: managerRolesWithGroupManagerAndSA,
			claims: [
				UserClaims.ManageHRModule,
				UserClaims.ManageAccountant,
				UserClaims.ManageProjectCoordinator,
			],
		},
	},
	{
		id: 16,
		label: 'Settings',
		icon: <SettingsIcon />,
		payload: {
			slug: Routes.settings,
			roles: managerRolesWithGroupManager,
			claims: [UserClaims.ManageAbsenceSettings],
		},
	},
];

const labboardSectionTabs: NavigationItem[] = [
	{
		id: 17,
		label: 'Labboard',
		icon: <CoursesIcon />,
		payload: {
			slug: Routes.labboard,
			roles: AllRoles,
			claims: [UserClaims.ManageInterns, UserClaims.ManageAllInterns],
		},
	},
];

export const tabsConfig: NavigationItem[][] = [
	firstSectionTabs,
	secondSectionTabs,
	labboardSectionTabs,
	thirdSectionTabs,
	fourthSectionTabs,
];

export const closedDrawerWidth = 75;

export function getTabIdBySlug(
	slug: string,
	tabs: NavigationItem[]
): number | null {
	const id = tabs.find((tab) => slug.startsWith(tab.payload?.slug || ''))?.id;

	return id || null;
}

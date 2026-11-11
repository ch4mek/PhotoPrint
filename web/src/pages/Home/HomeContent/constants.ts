import { AllRoles, allRolesExceptIntern, Roles } from '@/constants/roles';

export const HOME_MOBILE_BREAKPOINT = 930;
export const HOME_MOBILE_S_BREAKPOINT = HOME_MOBILE_BREAKPOINT / 2;
export const IS_PROGRAMMER_DAY_GAME_ENABLED = true;

export const homeWidgetsAccessRoles: Record<string, Roles[]> = {
	timesheet: AllRoles,
	developmentPlan: allRolesExceptIntern,
	upcomingEvent: allRolesExceptIntern,
	companyLinks: AllRoles,
	birthdays: allRolesExceptIntern,
	absences: allRolesExceptIntern,
};

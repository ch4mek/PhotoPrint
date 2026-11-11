import { lazy } from 'react';

import { personalInfoPopupUrlClaims } from '@/constants/claims';
import { AllRoles, employees } from '@/constants/roles';
import { PopUpSearchParams } from '@/constants/searchParams';

import { SearchParamsConfig } from './types';

const TeamboardUserPage = lazy(async () => ({
	default: (await import('@/pages/TeamBoardUser')).TeamboardUserPage,
}));

const ViewAbsenceRequestForm = lazy(async () => ({
	default: (await import('@/forms/Absence/ViewAbsenceRequestForm'))
		.ViewAbsenceRequestForm,
}));

export const searchParamsConfig: SearchParamsConfig = {
	[PopUpSearchParams.UserIdParam]: {
		PopUp: TeamboardUserPage,
		isAuthRequired: true,
		roles: AllRoles.filter((role) => !employees.includes(role)),
		claims: personalInfoPopupUrlClaims,
		requireNotOwnProfile: true,
	},
	[PopUpSearchParams.InquiryIdParam]: {
		PopUp: ViewAbsenceRequestForm,
		isAuthRequired: true,
		roles: AllRoles,
		claims: [],
	},
};

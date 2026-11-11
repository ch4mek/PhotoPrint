import { AbsenceFormValues } from './types';

export enum AbsenceTabs {
	Details,
	Approvals,
}

export const absenceTabsLabels = {
	[AbsenceTabs.Approvals]: 'Approvals',
	[AbsenceTabs.Details]: 'Details',
};

const initialAcceptPolicesValue = [
	{
		orderWeight: 1,
		scope: null,
		userId: null,
		substituteScope: null,
		substituteUserId: null,
		user: null,
		substituteUser: null,
	},
];

export const initialFormValues: AbsenceFormValues = {
	details: {
		name: '',
		type: null,
		trackingUnitType: null,
		activityType: null,
		accrualType: null,
		transferType: null,
		comment: '',
		isProhibitOnProbationPeriod: false,
		isIncludeWeekends: false,
		isIncludeHolidays: false,
		isRequireAttachments: false,
		acceptPolicies: [],
		agreements: [],
	},
	approvals: {
		isSubstitutesToApproveAllowed: false,
		acceptPolicies: initialAcceptPolicesValue,
	},
};

export const FORM_NAME = 'Absence form';

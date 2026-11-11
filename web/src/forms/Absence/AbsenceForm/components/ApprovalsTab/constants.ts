const fieldNames = {
	isSubstitutesToApproveAllowed: 'isSubstitutesToApproveAllowed',
};

export enum RoleScopes {
	Scope = 'scope',
	SubstituteScope = 'substituteScope',
	User = 'user',
	SubstituteUser = 'substituteUser',
}

export const initialAcceptPolicesValue = [
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

export const absenceApprovalsFieldData = {
	isSubstitutesToApproveAllowed: {
		key: fieldNames.isSubstitutesToApproveAllowed,
		name: fieldNames.isSubstitutesToApproveAllowed,
		label: 'Allow substitutes to approve',
		description:
			'If this option is checked, approval requests from approvers on leave will be forwarded to the selected substitute',
		valueField: 'name',
	},
};

export const MOBILE_L_BREAKPOINT = 737;
export const ICON_SIZE = 24;

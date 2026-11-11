import { AddGroupMemberFormFields, AddGroupMemberFormValues } from './types';

export const initialAddGroupMemberFormValues: AddGroupMemberFormValues = {
	user: null,
};

export const ADD_GROUP_MEMBER_FORM_LABEL = 'Group member adding';

export const USER_ADDED_TOAST_CONTENT =
	'has been successfully added to the group';

export const FIELD_LABELS = {
	[AddGroupMemberFormFields.USER]: 'User',
};

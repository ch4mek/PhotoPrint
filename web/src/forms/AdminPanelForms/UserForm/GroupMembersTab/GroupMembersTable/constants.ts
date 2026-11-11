import { GroupMembersTableColumnWidths } from './types';

export const GROUP_MEMBERS_TABLE_COLUMN_WIDTHS: GroupMembersTableColumnWidths =
	{
		userId: {
			width: 67,
			fixed: true,
		},
		userName: 200,
		email: 260,
		location: {
			width: 100,
			fixed: true,
		},
	};

export const GROUP_MEMBERS_TABLE_COLUMNS = {
	userId: {
		key: 'id',
		name: 'Id',
		sortable: false,
	},
	userName: {
		key: 'fullName',
		name: 'User name',
		sortable: false,
	},
	email: {
		key: 'email',
		name: 'Email',
		sortable: false,
	},
	location: {
		key: 'location',
		name: 'Location',
		sortable: false,
	},
};

export const CLEAR_FILTERS_COLUMN_INDEX = 6;

export const REVOKE_POPUP_LABEL = 'Group member removing';
export const REVOKE_WARNING_TEXT = ' will be removed from group.';

export const USER_REVOKED_TOAST_CONTENT =
	'has been successfully removed from the group';

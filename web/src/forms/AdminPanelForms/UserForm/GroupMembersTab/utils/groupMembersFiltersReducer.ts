import { DEFAULT_GROUP_MEMBERS_FILTER_VALUES } from '../constants';

import {
	GroupMembersAction,
	GroupMembersActionKeys,
	GroupMembersTableFilters,
} from '@/types/groupMembers';

export const groupMembersFiltersReducer = (
	state: GroupMembersTableFilters,
	{ type, payload }: GroupMembersAction
): GroupMembersTableFilters => {
	switch (type) {
		case GroupMembersActionKeys.set:
			return payload as GroupMembersTableFilters;
		case GroupMembersActionKeys.userId:
			return { ...state, userId: payload as string };
		case GroupMembersActionKeys.userName:
			return { ...state, userName: payload as string };
		case GroupMembersActionKeys.email:
			return { ...state, email: payload as string };
		case GroupMembersActionKeys.location:
			return { ...state, location: payload as string[] };
		case GroupMembersActionKeys.clear:
			return { ...DEFAULT_GROUP_MEMBERS_FILTER_VALUES };
		default:
			return state;
	}
};

import { Dispatch } from 'react';

import {
	GroupMembersAction,
	GroupMembersActionKeys,
	GroupMembersTableFilters,
	GroupMembersTableFilterSetters,
} from '@/types/groupMembers';

export const generateSetFilters = (
	dispatch: Dispatch<GroupMembersAction>
): GroupMembersTableFilterSetters => ({
	userId: (userId: string) =>
		dispatch({ type: GroupMembersActionKeys.userId, payload: userId }),
	userName: (userName: string) =>
		dispatch({ type: GroupMembersActionKeys.userName, payload: userName }),
	email: (email: string) =>
		dispatch({ type: GroupMembersActionKeys.email, payload: email }),
	location: (location: string[]) =>
		dispatch({ type: GroupMembersActionKeys.location, payload: location }),
	set: (newFilters: GroupMembersTableFilters) =>
		dispatch({ type: GroupMembersActionKeys.set, payload: newFilters }),
	clear: () => dispatch({ type: GroupMembersActionKeys.clear }),
});

import { DEFAULT_GROUP_MEMBERS_FILTER_VALUES } from '../constants';

import { GroupMembersTableFilters } from '@/types/groupMembers';

export const checkForEmptyFilter = (currentFilter: GroupMembersTableFilters) =>
	JSON.stringify(currentFilter) ===
	JSON.stringify(DEFAULT_GROUP_MEMBERS_FILTER_VALUES);

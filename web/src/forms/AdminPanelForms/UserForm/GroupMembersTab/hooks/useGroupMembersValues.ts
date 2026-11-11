import { useMemo } from 'react';

import { GroupMembersRow } from '../GroupMembersTable/types';

import { GroupUsersContract } from '@/queries/groups/types';

export const useGroupMembersValues = (
	members: Array<GroupUsersContract>
): Array<GroupMembersRow> =>
	useMemo<Array<GroupMembersRow>>(
		() =>
			members.map(({ email, fullName, location, id }) => ({
				id,
				fullName,
				email,
				location,
			})),
		[members]
	);

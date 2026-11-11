import { useMemo, useState } from 'react';

import { TabHeader } from '@/components/TabHeader';
import { AddGroupMemberForm } from '@/forms/AddGroupMemberForm';
import { useGetDepartmentUsersWithoutGroup } from '@/queries/groups/useGetDepartmentUsersWithoutGroup';

import { GroupInfoContext } from './context/GroupInfoContext';
import { GroupMembersTable } from './GroupMembersTable';
import { useGroupMembersFilters } from './hooks/useGroupMembersFilters';
import { GroupMembersTabWrapper } from './styles';
import { GroupInfoContextValues } from './types';

export function GroupMembersTab({
	groupId,
	departmentId,
}: {
	groupId: number;
	departmentId: string;
}) {
	const { filters, setFilters } = useGroupMembersFilters();
	const [isAddingGroupMember, setIsAddingGroupMember] = useState(false);
	const [isFetchingGroupMembers, setIsFethcingGroupMembers] = useState(false);

	const { data: usersWithoutGroups } = useGetDepartmentUsersWithoutGroup({
		departmentId,
	});

	const groupInfo = useMemo<GroupInfoContextValues>(
		() => ({ groupId, departmentId }),
		[groupId, departmentId]
	);

	const handleAddGroupMemberButtonClick = () => setIsAddingGroupMember(true);

	const handleAddGroupMemberPopupClose = () => setIsAddingGroupMember(false);

	return (
		<GroupMembersTabWrapper>
			<GroupInfoContext.Provider value={groupInfo}>
				<TabHeader
					onAddClick={handleAddGroupMemberButtonClick}
					isButtonDisabled={isFetchingGroupMembers}
				/>
				<GroupMembersTable
					filters={filters}
					filterSetters={setFilters}
					onGroupMembersFetching={setIsFethcingGroupMembers}
				/>
				{isAddingGroupMember && (
					<AddGroupMemberForm
						onClose={handleAddGroupMemberPopupClose}
						options={{ user: usersWithoutGroups ?? [] }}
					/>
				)}
			</GroupInfoContext.Provider>
		</GroupMembersTabWrapper>
	);
}

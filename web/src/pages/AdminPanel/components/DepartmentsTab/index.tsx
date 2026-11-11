import { useState } from 'react';

import { Loader } from '@/components/Loader';
import { GroupDeleteForm } from '@/forms/Departments/GroupDeleteForm';
import { GroupMutationForm } from '@/forms/Departments/GroupMutationForm';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { Group, GroupMutationType } from '@/types/department';

import { DepartmentsTable } from './components/DepartmentTable';
import { useGetDepartmentsWithGroupManagersFlag } from './hooks/useGetDepartmentsWithGroupManagersFlag';
import { useRowMenuOptions } from './hooks/useRowMenuOptions';
import { DepartmentsTabWrapper } from './styles';

export function DepartmentsTab() {
	const { data, isLoading, refetch } = useGetDepartmentsWithGroupManagersFlag();
	const [mutationType, setMutationType] = useState(GroupMutationType.None);

	const {
		id: groupId,
		isOpen: isGroupAddOrEditModalOpen,
		handleOpen: handleOpenGroupMutationModal,
		handleClose: handleCloseGroupMutationModal,
	} = useShowModal<Group>();
	const {
		id: deleteGroupId,
		isOpen: isGroupDeleteModalOpen,
		handleOpen: handleOpenGroupDeleteModal,
		handleClose: handleCloseGroupDeleteModal,
	} = useShowModal<Group>();
	const menuOptions = useRowMenuOptions(
		handleOpenGroupMutationModal,
		handleOpenGroupDeleteModal,
		setMutationType
	);

	return (
		<>
			<DepartmentsTabWrapper>
				<Loader isLoading={isLoading}>
					<DepartmentsTable {...{ data, menuOptions }} />
				</Loader>
			</DepartmentsTabWrapper>
			{isGroupAddOrEditModalOpen && (
				<GroupMutationForm
					id={groupId?.id}
					departmentId={groupId?.departmentId}
					handleClose={handleCloseGroupMutationModal}
					refetch={refetch}
					mutationType={mutationType}
				/>
			)}
			{isGroupDeleteModalOpen && deleteGroupId && (
				<GroupDeleteForm
					id={deleteGroupId.id}
					handleClose={handleCloseGroupDeleteModal}
					refetch={refetch}
				/>
			)}
		</>
	);
}

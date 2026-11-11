import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { PlusIcon } from '@mms/mms-ui-library/icons/base/PlusIcon';
import { Dispatch, SetStateAction, useMemo } from 'react';

import { PLUS_ICON_VIEW_BOX } from '../constants';
import { RowsMenuOptions } from '../types';

import { Group, GroupMutationType } from '@/types/department';

export function useRowMenuOptions(
	handleOpenGroupMutationModal: (item: Group | undefined) => void,
	handleOpenGroupDeleteModal: () => void,
	setMutationType: Dispatch<SetStateAction<GroupMutationType>>
): RowsMenuOptions {
	const departmentRowMenuOptions = useMemo<PickerButtonOption[]>(
		() => [
			{
				value: 'Add group',
				addon: <PlusIcon viewBox={PLUS_ICON_VIEW_BOX} />,
				onClick: (item) => {
					setMutationType(GroupMutationType.Add);
					handleOpenGroupMutationModal(item);
				},
			},
		],
		[]
	);

	const groupRowMenuOptions = useMemo<PickerButtonOption[]>(
		() => [
			{
				value: 'Edit',
				addon: <EditIcon />,
				onClick: (item) => {
					setMutationType(GroupMutationType.Edit);
					handleOpenGroupMutationModal(item);
				},
			},
			{
				value: 'Delete',
				addon: <BasketIcon />,
				onClick: handleOpenGroupDeleteModal,
			},
		],
		[]
	);

	return {
		departmentRowMenuOptions,
		groupRowMenuOptions,
	};
}

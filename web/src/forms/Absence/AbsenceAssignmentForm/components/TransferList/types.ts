import { TransferListItem } from '@/forms/Absence/AbsenceAssignmentForm/types';

export interface TransferListProps {
	unselectedItems: TransferListItem[];
	selectedItems: TransferListItem[];
	setSelectedItems: (selectedUsers: TransferListItem[]) => void;
	setUnselectedItems: (unselectedUsers: TransferListItem[]) => void;
	searchValue: string;
	checkedItems: TransferListItem[];
	setCheckedItems: (selectedUsers: TransferListItem[]) => void;
}

export enum ListLabelType {
	Unassigned = 'Unassigned users',
	Assigned = 'Assigned users',
}

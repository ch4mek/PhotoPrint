import { TransferListItem } from '@/forms/Absence/AbsenceAssignmentForm/types';

export interface ListProps {
	items: TransferListItem[];
	handleCheckboxClick: (id: TransferListItem) => () => void;
	checkIsChecked: (id: number) => boolean;
}

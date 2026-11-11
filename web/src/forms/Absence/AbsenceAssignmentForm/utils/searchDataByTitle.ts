import { TransferListItem } from '@/forms/Absence/AbsenceAssignmentForm/types';

export const searchDataByTitle = (items: TransferListItem[], query: string) => {
	if (!query.trim()) {
		return items;
	}

	const lowerQuery = query.toLowerCase();

	return items.filter((item) => item.title.toLowerCase().includes(lowerQuery));
};

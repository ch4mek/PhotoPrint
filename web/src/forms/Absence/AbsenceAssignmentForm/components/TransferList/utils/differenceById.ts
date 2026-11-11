import { TransferListItem } from '@/forms/Absence/AbsenceAssignmentForm/types';

export const differenceById = (
	source: TransferListItem[],
	target: TransferListItem[]
) => {
	const targetIds = new Set(target.map((item) => item.id));

	return source.filter((item) => !targetIds.has(item.id));
};

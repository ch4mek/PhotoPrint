import { AssignmentUser } from '@/types/absences';

export const convertResponseToTransferListData = (data?: AssignmentUser[]) => {
	if (!data) {
		return [];
	}

	return data.map(({ id, fullName }) => ({
		id,
		title: fullName,
	}));
};

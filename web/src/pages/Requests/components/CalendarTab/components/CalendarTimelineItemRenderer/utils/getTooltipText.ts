import { GetTooltipTextParams } from '../types';

import { AbsenceRequestStatus } from '@/types/absences';

export const getTooltipText = ({
	inquiry: { inquiryName, inquiryStatus },
	totalHours,
	alreadyApprovedCount,
	totalApproversCount,
}: GetTooltipTextParams): string => {
	const parts = [inquiryName];

	if (totalHours) {
		parts.push(`${totalHours}h`);
	}

	const statusPart =
		inquiryStatus === AbsenceRequestStatus.Open
			? `${inquiryStatus} ${alreadyApprovedCount}/${totalApproversCount}`
			: inquiryStatus;

	parts.push(statusPart);

	return parts.join(' - ');
};

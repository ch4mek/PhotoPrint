import { getFullName } from '@/helpers/utils';
import { UserInquiryStatusByApprover } from '@/types/absences';

export const getTooltipLabel = (
	status: UserInquiryStatusByApprover,
	firstName?: string,
	lastName?: string
) => `${status} by ${getFullName(firstName || '', lastName || '')}`;

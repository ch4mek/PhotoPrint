import { TFunction } from 'i18next';

import { getFullName } from '@/helpers/utils';
import {
	UserInquiryApprover,
	UserInquiryStatusByApprover,
} from '@/types/absences';

export const getTooltipText = (
	status: UserInquiryStatusByApprover,
	approver: UserInquiryApprover | null,
	t: TFunction<'common'>
) => {
	const { firstName, lastName } = approver ?? {
		firstName: '',
		lastName: '',
	};

	const fullName = getFullName(firstName, lastName);

	switch (status) {
		case UserInquiryStatusByApprover.Accepted:
			return `${t('absence.approvedBy')} ${fullName}`;
		case UserInquiryStatusByApprover.Rejected:
			return `${t('absence.rejectedBy')} ${fullName}`;
		default:
			return fullName;
	}
};

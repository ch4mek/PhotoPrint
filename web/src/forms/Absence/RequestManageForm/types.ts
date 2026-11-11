import { Resources } from 'i18next';

import { ManageRequestType, UserInquiryApprover } from '@/types/absences';

interface RequestManageFormItem {
	requestName: string;
	id: number;
	createdForUser: UserInquiryApprover;
}

export interface RequestManageFormProps {
	type: ManageRequestType;
	item: RequestManageFormItem;
	handleClose: VoidFunction;
}
type AbsenceMessageKey = keyof Resources['common']['absence'];
export type MessageLabelByType = Record<
	ManageRequestType,
	{ toast: AbsenceMessageKey; title: AbsenceMessageKey }
>;

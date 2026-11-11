import {
	UserInquiryApprover,
	UserInquiryStatusByApprover,
	AllRolesOptions,
} from '@/types/absences';

export interface ApproverItemProps {
	item: UserInquiryApprover;
	isDisabled: boolean;
	status: UserInquiryStatusByApprover;
	approver?: UserInquiryApprover;
	acceptUserPosition: string;
	acceptPolicyScope: AllRolesOptions;
	inquiryId: number;
	inquiryAcceptId: number;
}

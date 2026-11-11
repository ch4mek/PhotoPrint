import { UserInquiryApprover } from '@/types/absences';

export interface RequestParticipantsProps {
	requestFromData?: UserInquiryApprover;
	managerData?: UserInquiryApprover | null;
	createdData?: UserInquiryApprover;
}

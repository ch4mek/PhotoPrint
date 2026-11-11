import { UserInquiryAccepts } from '@/types/absences';

export interface RequestApproversProps {
	data: UserInquiryAccepts[];
	inquiryId: number;
}

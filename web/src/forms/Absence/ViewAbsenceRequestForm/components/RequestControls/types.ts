import { AbsenceType, ManageRequestType } from '@/types/absences';

export interface RequestControlsProps {
	canCurrentUserApprove?: boolean | null;
	canCurrentUserReject?: boolean | null;
	handleOpen: (type: ManageRequestType) => void;
	inquiryType: AbsenceType;
}

import { TeamboardUserAccessLevel } from '@/pages/TeamBoardUser/types';
import { UserAbsenceItem } from '@/types/absences';

export interface ModalData {
	templateId: number | string;
	formType: string;
}

export interface UserAbsenceItemProps {
	absenceItem: UserAbsenceItem;
	handleOpen: (data: ModalData) => void;
	userId?: number | string;
	accessLevel?: TeamboardUserAccessLevel;
}

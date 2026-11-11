import { TeamboardUserAccessLevel } from '@/pages/TeamBoardUser/types';
import { UserAbsenceItem } from '@/types/absences';

export interface AbsenceItemListProps {
	data: UserAbsenceItem[];
	userId: number | undefined;
	locationId: number | null;
	accessLevel?: TeamboardUserAccessLevel;
}

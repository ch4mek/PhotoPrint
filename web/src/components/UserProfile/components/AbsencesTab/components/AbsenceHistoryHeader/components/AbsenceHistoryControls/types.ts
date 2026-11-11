import { UserAbsenceItem } from '@/types/absences';

export interface AbsenceHistoryControlsProps {
	absences: UserAbsenceItem[];
	totalUsed: number | null;
}

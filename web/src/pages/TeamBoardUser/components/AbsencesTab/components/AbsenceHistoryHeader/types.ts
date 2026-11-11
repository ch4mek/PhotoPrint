import { UserAbsenceItem } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export interface AbsenceHistoryHeaderProps {
	absences: UserAbsenceItem[];
	totalUsed: number | null;
}

export interface UseGetAbsenceTypeTotalCountProps {
	absenceFilter: SimpleConfigurableConfigurationItem;
	absences: UserAbsenceItem[];
	totalUsed: number | null;
}

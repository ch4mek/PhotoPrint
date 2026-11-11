import { Absence } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export interface ItemContentProps {
	item: Absence;
	handleAbsenceAssigns?: (
		item: SimpleConfigurableConfigurationItem<number>
	) => void;
}

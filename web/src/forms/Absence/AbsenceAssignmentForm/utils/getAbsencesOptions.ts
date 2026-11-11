import { Absence } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export const getAbsencesOptions = (
	absenceItems?: Absence[]
): SimpleConfigurableConfigurationItem<number>[] =>
	absenceItems?.map(({ id, name }) => ({
		id: +id,
		name,
	})) || [];

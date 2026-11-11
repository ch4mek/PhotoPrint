import { Absence } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export const getAgreementOptions = (
	selectedAbsenceId: number | null,
	absenceItems?: Absence[]
): SimpleConfigurableConfigurationItem<number>[] => {
	const selectedAbsence = absenceItems?.find(
		(item) => item.id === selectedAbsenceId
	);

	return (
		selectedAbsence?.agreements?.map(({ id, name, isDefault }) => ({
			id: +id,
			name: `${name}${isDefault ? ' (Default)' : ''}`,
		})) || []
	);
};

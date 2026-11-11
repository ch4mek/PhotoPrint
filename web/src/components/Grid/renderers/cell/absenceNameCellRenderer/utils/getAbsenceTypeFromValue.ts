import { absencesType } from '@/constants/absence';
import { AbsenceType } from '@/types/absences';

export const getAbsenceTypeFromValue = (value: string): AbsenceType | null => {
	if (!value) {
		return null;
	}

	const normalizedValue = value.trim().toLowerCase();

	const entry = Object.entries(absencesType).find(
		([, label]) => label.toLowerCase() === normalizedValue
	);

	return entry ? (entry[0] as AbsenceType) : null;
};

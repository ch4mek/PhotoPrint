import { AbsenceRow, AbsenceType, UserAbsenceItem } from '@/types/absences';

export const mapAbsencesToRows = (
	items: Array<UserAbsenceItem>
): Array<AbsenceRow> =>
	items
		.filter(
			(absence) =>
				absence.type === AbsenceType.SickDay ||
				absence.type === AbsenceType.Vacation
		)
		.map(({ inquiryTemplateId, trackingUnitCount, name }) => ({
			id: inquiryTemplateId,
			name,
			availableDays: trackingUnitCount,
		}));

import { AbsenceType, UserAbsenceItem } from '@/types/absences';

export const findMatchingAbsence = (
	absences: UserAbsenceItem[] | undefined,
	inquiryTemplateId: string | number
): UserAbsenceItem | undefined =>
	absences?.find(
		(absence) =>
			absence.type === AbsenceType.Documents &&
			absence.inquiryTemplateId === inquiryTemplateId
	);

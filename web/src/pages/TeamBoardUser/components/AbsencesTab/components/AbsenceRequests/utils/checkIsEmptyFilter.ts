import { defaultAbsenceRequestFilterValues } from '@/constants/absence';
import { AbsenceRequestFilters } from '@/types/absences';

export const checkIsEmptyFilter = (filters: AbsenceRequestFilters) =>
	JSON.stringify(filters) === JSON.stringify(defaultAbsenceRequestFilterValues);

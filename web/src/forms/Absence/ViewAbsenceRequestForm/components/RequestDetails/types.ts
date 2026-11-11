import { Holiday } from '@mms/mms-ui-library/components/CalendarRange';

import { AbsenceRequest } from '@/types/absences';

export interface RequestDetailsProps {
	holidays: Holiday[];
	item?: AbsenceRequest;
}

import { Holiday } from '@mms/mms-ui-library/components/CalendarRange';

import { AbsenceRequest } from '@/types/absences';

export type RequestCalendarProps = Pick<
	AbsenceRequest,
	'dateFrom' | 'dateTo'
> & {
	holidays: Holiday[];
};

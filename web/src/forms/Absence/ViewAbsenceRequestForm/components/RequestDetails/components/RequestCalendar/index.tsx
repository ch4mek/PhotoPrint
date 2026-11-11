import { CalendarRange } from '@mms/mms-ui-library/components/CalendarRange';

import dayjs from '@/lib/dayjs';

import { CalendarRangeWrapper, RequestCalendarWrapper } from './styles';
import { RequestCalendarProps } from './types';

export function RequestCalendar({
	dateTo,
	dateFrom,
	holidays,
}: RequestCalendarProps) {
	return (
		<RequestCalendarWrapper>
			<CalendarRange
				range={[dayjs(dateFrom).valueOf(), dayjs(dateTo).valueOf()]}
				isStatic
				calendarRangeWrapper={CalendarRangeWrapper}
				{...{ holidays }}
			/>
		</RequestCalendarWrapper>
	);
}

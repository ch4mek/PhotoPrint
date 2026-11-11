import { DATE_FORMAT } from '@/constants/date';
import { assertNever } from '@/helpers/assertNever';
import dayjs from '@/lib/dayjs';
import {
	EventsWidgetRow,
	EmployeeEventType,
	EmployeeEvent,
} from '@/types/dashboard';

import { EVENT_TYPE_TEXT } from './constants';

const getEventTypeName = (
	eventType: EmployeeEventType,
	yearsCount: number
): string => {
	switch (eventType) {
		case EmployeeEventType.Birthday:
			return EVENT_TYPE_TEXT.birthday;
		case EmployeeEventType.EmploymentAnniversary:
			return yearsCount === 1
				? EVENT_TYPE_TEXT.yearSingular
				: EVENT_TYPE_TEXT.yearPlural(yearsCount);
		default:
			return assertNever(eventType);
	}
};

export const mapEventToRow = ({
	userId,
	fullName,
	eventType,
	eventCelebrationDate,
	originalEventDate,
}: EmployeeEvent): EventsWidgetRow => {
	const yearsCount =
		new Date(eventCelebrationDate).getFullYear() -
		new Date(originalEventDate).getFullYear();

	return {
		id: `${userId}-${eventType}`,
		userId,
		fullName,
		eventType: getEventTypeName(eventType, yearsCount),
		eventDate: dayjs(eventCelebrationDate).format(DATE_FORMAT),
	};
};

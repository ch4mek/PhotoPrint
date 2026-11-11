import i18next, { TFunction } from 'i18next';

import { createPeriodInText } from '@/helpers/booking';
import { getFormattedDateTimePeriodWithUTC } from '@/helpers/bookingFormatters';
import { MeetingRoomsCollision } from '@/queries/booking/types';

export const getMeetingRoomBookedForPeriod = (
	meetingRoomCollision: MeetingRoomsCollision,
	t: TFunction<'booking'>
) => {
	const {
		reservationFromOfficeDateTime,
		reservationToOfficeDateTime,
		meetingRoomName,
	} = meetingRoomCollision;

	const period = `${createPeriodInText(0, [
		reservationFromOfficeDateTime,
	])}, ${getFormattedDateTimePeriodWithUTC(
		reservationFromOfficeDateTime,
		reservationToOfficeDateTime,
		i18next.getFixedT(null, 'common')
	)}`;

	return t('warnings.meetingRoomCollision', {
		room: meetingRoomName,
		period,
	});
};

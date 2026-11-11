import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useTranslation } from 'react-i18next';

import { PopUpBody } from '../PopUpBody';

import { createPeriodInText } from '@/helpers/booking';
import { getFormattedDateTimePeriodWithUTC } from '@/helpers/bookingFormatters';
import { getDeskName, getSpaceAbbreviation } from '@/pages/Booking/utils';
import {
	MeetingRoomBookingCancelledNotification,
	NotificationType,
	WorkplaceBookingCancelledNotification,
} from '@/types/notifications';

import { BookingCancelledPopupBodyProps } from './types';

export function BookingCancelledPopupBody({
	notification,
}: BookingCancelledPopupBodyProps) {
	const { t } = useTranslation('common');

	if (
		notification.notificationType === NotificationType.WorkplaceBookingCancelled
	) {
		const {
			WorkplaceNumber,
			Message,
			DateList,
			WorkplaceOfficeId,
			WorkspaceId,
		} = notification as WorkplaceBookingCancelledNotification;

		return (
			<PopUpBody>
				<Typography variant="m-400">{Message}</Typography>
				<Typography variant="m-600">
					{getDeskName(
						getSpaceAbbreviation(WorkplaceOfficeId, WorkspaceId),
						WorkplaceNumber
					)}
				</Typography>
				<Typography variant="m-600">
					{createPeriodInText(0, DateList, t, false, true)}
				</Typography>
			</PopUpBody>
		);
	}

	if (
		notification.notificationType ===
		NotificationType.MeetingRoomBookingCancelled
	) {
		const { MeetingRoomName, FromTime, ToTime, Message, DateList } =
			notification as MeetingRoomBookingCancelledNotification;

		return (
			<PopUpBody>
				<Typography variant="m-400">{Message}</Typography>
				<Typography variant="m-600">{MeetingRoomName}</Typography>
				<Typography variant="m-600">
					{createPeriodInText(0, DateList, t, false, true)}
				</Typography>
				<Typography variant="m-600">
					{getFormattedDateTimePeriodWithUTC(FromTime, ToTime, t)}
				</Typography>
			</PopUpBody>
		);
	}

	return null;
}

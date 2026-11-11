import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useTranslation } from 'react-i18next';

import { PopUpBody } from '../PopUpBody';

import { useAuth } from '@/context/AuthContext';
import { createPeriodInText, getDeleteDialogText } from '@/helpers/booking';
import { getFormattedTimePeriod } from '@/helpers/bookingFormatters';
import { getDialogTextComponent } from '@/helpers/getDeleteDialogTextComponent';
import { getMeetingRoomDateList } from '@/pages/Booking/utils';

import { DeleteBookingPopupProps } from './types';

export function DeleteMeetingRoomBookingPopupBody({
	bookingItem,
}: DeleteBookingPopupProps) {
	const {
		weeksInInterval,
		reservationPeriodFromOfficeDate,
		reservationPeriodToOfficeDate,
		userFirstName,
		userLastName,
		meetingRoomName,
		reservationFromOfficeTime,
		reservationToOfficeTime,
		isRepetitiveBooking,
		userId = 0,
	} = bookingItem;
	const { id } = useAuth();
	const { t: tBooking } = useTranslation('booking');
	const { t } = useTranslation('common');

	const deskLikeBookingItem = {
		isBlock: false,
		userId,
		firstName: userFirstName,
		lastName: userLastName,
	};

	const dialogText = getDeleteDialogText(deskLikeBookingItem, id, tBooking);
	const dialogTextComponent = getDialogTextComponent(
		deskLikeBookingItem,
		id,
		tBooking
	);

	return (
		<PopUpBody>
			{dialogText && <Typography variant="m-400">{dialogText}</Typography>}
			{dialogTextComponent && (
				<Typography variant="m-400">{dialogTextComponent}</Typography>
			)}
			<Typography variant="m-600">{meetingRoomName}</Typography>
			<Typography variant="m-600">
				{createPeriodInText(
					weeksInInterval,
					getMeetingRoomDateList(
						reservationPeriodFromOfficeDate,
						reservationPeriodToOfficeDate,
						isRepetitiveBooking
					),
					t,
					false,
					true
				)}
			</Typography>
			<Typography variant="m-600">
				{getFormattedTimePeriod(
					reservationFromOfficeTime,
					reservationToOfficeTime,
					false,
					t
				)}
			</Typography>
		</PopUpBody>
	);
}

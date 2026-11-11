import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useTranslation } from 'react-i18next';

import { PopUpBody } from '../PopUpBody';

import { useAuth } from '@/context/AuthContext';
import { createPeriodInText, getDeleteDialogText } from '@/helpers/booking';
import { getDialogTextComponent } from '@/helpers/getDeleteDialogTextComponent';
import { getDeskName, getSpaceAbbreviation } from '@/pages/Booking/utils';

import { DeleteBookingPopupProps } from './types';

export function DeleteDeskBookingPopupBody({
	bookingItem,
}: DeleteBookingPopupProps) {
	const {
		isBlock,
		workplaceNumber,
		weeksInInterval,
		dateList,
		workplaceOfficeId,
		workspaceId,
	} = bookingItem;
	const { id } = useAuth();
	const { t: tBooking } = useTranslation('booking');
	const { t } = useTranslation('common');

	const dialogComponent = getDialogTextComponent(bookingItem, id, tBooking);
	const dialogText = getDeleteDialogText(bookingItem, id, tBooking);

	return (
		<PopUpBody>
			{dialogText && <Typography variant="m-400">{dialogText}</Typography>}
			{dialogComponent && (
				<Typography variant="m-400">{dialogComponent}</Typography>
			)}
			{!isBlock && (
				<>
					<Typography variant="m-600">
						{getDeskName(
							getSpaceAbbreviation(workplaceOfficeId, workspaceId),
							workplaceNumber
						)}
					</Typography>
					<Typography variant="m-600">
						{createPeriodInText(weeksInInterval, dateList, t)}
					</Typography>
				</>
			)}
		</PopUpBody>
	);
}

import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useTranslation } from 'react-i18next';

import { SelectedBookingItemType } from '@/context/types';
import { getDeleteDialogHeader } from '@/helpers/booking';
import { DeleteBookingPopupProps } from '@/pages/Booking/components/PopUps/DeleteBookingPopup/types';
import { DeleteDeskBookingPopupBody } from '@/pages/Booking/components/PopUps/DeleteDeskBookingPopupBody';
import { DeleteMeetingRoomBookingPopupBody } from '@/pages/Booking/components/PopUps/DeleteMeetingRoomBookingPopupBody';

export function DeleteBookingPopup({
	item,
	handleClose,
	handleSubmit,
	isLoading,
}: DeleteBookingPopupProps) {
	const { t } = useTranslation('booking');
	const { t: tCommon } = useTranslation('common', {
		keyPrefix: 'modals.controls',
	});

	return (
		<PopUp
			title={getDeleteDialogHeader(item.isBlock, t)}
			onClose={handleClose}
			type="approve"
			showCloseButton={false}
			showLoader={isLoading}
			controls={{
				negativeControl: { onClick: handleClose, text: tCommon('no') },
				positiveControl: { onClick: handleSubmit, text: tCommon('yes') },
			}}
		>
			{item.type === SelectedBookingItemType.Room ? (
				<DeleteMeetingRoomBookingPopupBody bookingItem={item} />
			) : (
				<DeleteDeskBookingPopupBody bookingItem={item} />
			)}
		</PopUp>
	);
}

import {
	ConvertedDeleteDeskBookingItem,
	ConvertedDeleteMeetingRoomBookingItem,
} from '@/pages/Booking/components/BookingList/types';

export interface DeleteBookingPopupProps {
	item: ConvertedDeleteDeskBookingItem | ConvertedDeleteMeetingRoomBookingItem;
	handleClose: VoidFunction;
	handleSubmit: VoidFunction;
	isLoading: boolean;
}

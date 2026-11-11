import { SelectedBookingItemType } from '@/context/types';
import {
	GetBookingResponse,
	GetRoomsBookingResponse,
} from '@/queries/booking/types';
import { DeskBooking, MeetingRoomBooking } from '@/types/booking';

export interface BookingListProps {
	deskData?: GetBookingResponse;
	roomsData?: GetRoomsBookingResponse;
	showLoader: boolean;
}

export interface RoomBookingItems {
	type: SelectedBookingItemType.Room;
	items: Array<MeetingRoomBooking>;
}
export interface DeskBookingItems {
	type: SelectedBookingItemType.Desk;
	items: Array<DeskBooking>;
}

export interface ConvertedDeleteDeskBookingItem extends DeskBooking {
	type: SelectedBookingItemType.Desk;
}
export interface ConvertedDeleteMeetingRoomBookingItem
	extends MeetingRoomBooking {
	type: SelectedBookingItemType.Room;
	isBlock: false;
}

export interface BookingListItemProps {
	type: string;
	isNeedUnderline?: boolean;
}

export type RoomBookingEdgeMilliseconds = Record<
	'firstDate' | 'lastDate',
	number
>;
export type RoomBookingEdgeDates = Record<
	keyof RoomBookingEdgeMilliseconds,
	string
>;

import { DeskBooking, MeetingRoomBooking } from '@/types/booking';

export const isMeetingRoom = (
	item: DeskBooking | MeetingRoomBooking
): item is MeetingRoomBooking => 'reservations' in item;

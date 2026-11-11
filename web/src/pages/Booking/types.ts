import { DeskBooking, MeetingRoomBooking } from '@/types/booking';

export enum BookingTabsIndexes {
	BookingForm = 0,
	Workplace = 1,
}

export enum BookingOfficesId {
	Minsk = 1,
	Warsaw = 3,
}

export enum CancelType {
	Booking = 'booking',
	Blocking = 'blocking',
}

export interface UseDeleteBookingProps {
	id: number;
	item: DeskBooking | MeetingRoomBooking | null;
}

export type ParsedBookingItem =
	| DeskBooking
	| (Pick<MeetingRoomBooking, 'weeksInInterval' | 'userId'> & {
			dateList: string[];
			isBlock: boolean;
	  });

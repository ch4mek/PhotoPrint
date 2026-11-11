import { EquipmentItem } from '@/components/Timeline/types';
import { RequestTypes } from '@/constants/booking';
import { Dayjs } from '@/lib/dayjs';
import {
	DeskBooking,
	BookingReservation,
	MeetingRoomBooking,
} from '@/types/booking';

export interface CreateMutationVariables {
	requestType: RequestTypes;
	workplaceId: number;
	fromDate: Dayjs | null;
	toDate: Dayjs | null;
	weeksInterval?: number;
	userId: number | null | undefined;
	officeId: number;
}

export interface CreateGuestBookingVariables {
	requestType: RequestTypes;
	workplaceId: number;
	fromDate: Dayjs | null;
	toDate: Dayjs | null;
	weeksInterval?: number;
	guestName: string;
	officeId: number;
}

export interface CreateMeetingRoomBookingVariables {
	roomId: number;
	userId: number | null | undefined;
	reservationFromUtc: string;
	reservationToUtc: string;
	reservationPeriodFromOfficeDate: string;
	reservationPeriodToOfficeDate: string;
	repeatReservationWeeks: number;
}

export type DeleteMutationVariables = {
	id: number;
};

export type DeleteMeetingRoomVariables = {
	entryId: number;
};

export interface MeetingRoomReservationEntry {
	id: number;
	userId: number;
	userFirstName: string;
	userLastName: string;
	meetingRoomId: number;
	reservationFromUtc: string;
	reservationToUtc: string;
	reservationFromOfficeDateTime: string;
	reservationToOfficeDateTime: string;
	isAllDay: boolean;
	comments: string;
	meetingRoomEntryId: number;
	isMeetingRoomEntryRepetitiveBooking: boolean;
	meetingRoomEntryWeeksInInterval: number;
}

export interface MeetingRoomReservation {
	meetingRoomId: number;
	meetingRoomName: string;
	reservations: Array<MeetingRoomReservationEntry>;
	countOfSeats: number;
	equipments: Array<EquipmentItem>;
}

export type GetBookingResponse = {
	bookings: Array<DeskBooking>;
};

export type GetRoomsBookingResponse = Array<MeetingRoomBooking>;

export type GetReservationResponse = Array<BookingReservation>;

export type GetMeetingRoomsReservationsResponse = Array<MeetingRoomReservation>;

export interface MeetingRoomsCollision {
	id: number;
	userId: number;
	userFirstName: string;
	userLastName: string;
	meetingRoomId: number;
	meetingRoomName: number;
	meetingRoomEntryId: number;
	reservationFromUtc: string;
	reservationToUtc: string;
	reservationFromOfficeDateTime: string;
	reservationToOfficeDateTime: string;
	isAllDay: boolean;
	comments: string;
}
export type GetMeetingRoomsCollisions = Array<MeetingRoomsCollision>;

export interface WorkplaceInfo {
	id: number;
	workspaceId: number | null;
	workplaceNumber: number;
	isDisabled: boolean;
}

export type GetSpaceWorkplacesConfig = Array<WorkplaceInfo>;

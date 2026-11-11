import { MeetingRoomReservationEntry } from '@/queries/booking/types';

import { LocationTypeId } from './configuration';

export enum DeskSize {
	Small = 'small',
	Large = 'large',
}

export interface BookingEntity {
	bookingId: number;
	userId: number;
	firstName: string;
	lastName: string;
	dateList: Array<string>;
	isBlock: boolean;
	workplaceNumber: number;
	workplaceId: number;
	weeksInInterval: number;
}

export interface DeskBooking {
	bookingId: number;
	userId: number;
	dateList: Array<string>;
	firstName: string;
	lastName: string;
	isBlock: boolean;
	workplaceId: number;
	workplaceNumber: number;
	workplaceOfficeId: number;
	workspaceId: number;
	weeksInInterval: number;
	guestName: string | null;
	bookedBy: string | null;
	isForGuest: boolean;
}

export interface MeetingRoomBooking {
	id: number;
	weeksInInterval: number;
	isRepetitiveBooking: boolean;
	reservationFromUtc: string;
	reservationToUtc: string;
	reservationPeriodFromOfficeDate: string;
	reservationPeriodToOfficeDate: string;
	reservationFromOfficeTime: string;
	reservationToOfficeTime: string;
	userId: number;
	userFirstName: string;
	userLastName: string;
	meetingRoomId: number;
	reservationEndOfficeDate: string;
	meetingRoomName: string;
	reservations: MeetingRoomReservationEntry[];
}

export interface Workplace {
	bookingId: number;
	workplaceNumber: number;
	workplaceId: number;
	userId: number;
	firstName: string;
	lastName: string;
	department: string;
	isBlocked: boolean;
	isDisabled: boolean;
	isForGuest: boolean;
	guestName: string | null;
}

export interface BookingReservation {
	date: string;
	userWorkplaces: Workplace[];
}

export enum MinskSpacesInternalCodes {
	SpaceA = 'MPN186-S403',
	SpaceB = 'MPN186-S404',
	SpaceC = 'MPN186-S405',
	SpaceD = 'MPN186-S406',
	SpaceE = 'MPN186-S411',
	SpaceF = 'MPN186-S207',
}

export interface OfficeSelectorOption {
	name: string;
	officeLocation: LocationTypeId;
}

export enum BookingModes {
	Employees,
	Guest,
}

export type Timestamp = number;

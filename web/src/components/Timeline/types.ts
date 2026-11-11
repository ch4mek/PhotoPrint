import {
	MeetingRoomReservation,
	MeetingRoomReservationEntry,
} from '@/queries/booking/types';
import { MeetingRoomBooking } from '@/types/booking';
import { MeetingRoomType } from '@/types/configuration';

export interface SelectedTimePeriod<T> {
	from: T;
	to: T;
	isOverlapped: boolean;
	isLoading: boolean;
}

export type SelectTimePeriod<T> = (section: Section, from: T, to: T) => void;

export interface BookedDataTime {
	hours: number;
	minutes: number;
}

export interface TimelineProps {
	data: BookedDataResponse[];
	selectedTimePeriod?: SelectedTimePeriod<BookedDataTime> | null;
	onTimePeriodSelection?: SelectTimePeriod<Date>;
	selectedSectionId?: number | null;
	selectedSectionName?: string | null;
	onSectionSelection?: (section: Section) => void;
	isWorkingHoursOnly?: boolean;
	onIsWorkingHoursOnlyChange?: (isWorkingHoursOnly: boolean) => void;
	selectedDate: Date;
	onGoForwardDate: () => void;
	onGoBackwardDate: () => void;
	onGoTodayDate: () => void;
	handleCancelClick?: VoidFunction;
	meetingRoomsConfig: Array<MeetingRoomType>;
	scrollToSelectedPeriod?: Pick<TimelineScroll, 'behavior'>;
	scrollToCurrentTime?: TimelineScroll;
	selectionPeriod?: number;
	selectFromPeriodStart?: boolean;
	hasCurrentTimeIndicator?: boolean;
	isPeriodSelectionDisabled?: boolean;
	showLoader?: boolean;
	timezone?: TimezoneObject;
	onSetItemToDelete?: (booking: MeetingRoomBooking | null) => void;
}

export interface BookedData
	extends Omit<
		MeetingRoomReservationEntry,
		| 'reservationFromUtc'
		| 'reservationToUtc'
		| 'reservationFromOfficeDateTime'
		| 'reservationToOfficeDateTime'
		| 'isMeetingRoomEntryRepetitiveBooking'
	> {
	id: number;
	fromDateTime: BookedDataTime;
	toDateTime: BookedDataTime;
	isRepetitive: boolean;
	handleCancelClick?: VoidFunction;
	isSelected?: boolean;
	showSkeleton?: boolean;
	selectedSectionName?: string | null;
}

export interface BookedDataResponse
	extends Omit<MeetingRoomReservation, 'reservations'> {
	reservations: BookedData[];
	countOfSeats: number;
	equipments: EquipmentItem[];
}

export interface EquipmentItem {
	id: number;
	name: string;
	type: MeetingRoomEquipmentTypes;
	count: number;
}

export interface Section {
	meetingRoomId: number;
	meetingRoomName: string;
	countOfSeats: number;
	equipments: EquipmentItem[];
}

type TimelineScrollBehaviour = 'smooth' | 'auto';
export type TinelineScrollPosition = 'start' | 'center' | 'end';

export interface TimelineScroll {
	behavior: TimelineScrollBehaviour;
	position?: TinelineScrollPosition;
}

export enum Timezone {
	MINSK = 'Europe/Minsk',
	WARSAW = 'Europe/Warsaw',
}

export enum MeetingRoomEquipmentTypes {
	SEATS = 'Seats',
	BOARD = 'Board',
	USBHUB = 'UsbHub',
	LAPTOP = 'Laptop',
	SCREEN = 'Screen',
	SPEAKERPHONE = 'Speakerphone',
	CAMERA = 'Camera',
}

export interface TimezoneObject {
	timezoneDisplayName: string;
	timezoneOffset: string;
	timezone: Timezone;
}

import {
	GetBookingResponse,
	GetRoomsBookingResponse,
} from '@/queries/booking/types';
import { OfficeType } from '@/types/configuration';

export interface UseBookingListProps {
	office: OfficeType;
}

export interface BookingsState {
	desks: GetBookingResponse | undefined;
	rooms: GetRoomsBookingResponse | undefined;
}

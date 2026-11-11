import { BookedData } from '../types';

import { MeetingRoomBooking } from '@/types/booking';

export type BookedLabelProps = BookedData & {
	onSetItemToDelete?: (booking: MeetingRoomBooking | null) => void;
};

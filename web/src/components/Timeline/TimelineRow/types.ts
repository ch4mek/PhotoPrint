import { TimelineGridProps } from '../TimelineGrid/types';
import { BookedData, Section, SelectedTimePeriod } from '../types';

import { MeetingRoomBooking } from '@/types/booking';

export interface TimelineRowProps
	extends Omit<TimelineGridProps, 'data' | 'selectedDate' | 'selectedPeriod'> {
	section: Section;
	bookedData: BookedData[];
	selectedPeriod: SelectedTimePeriod<number> | null;
	handleCancelClick?: VoidFunction;
	onSetItemToDelete?: (booking: MeetingRoomBooking | null) => void;
}

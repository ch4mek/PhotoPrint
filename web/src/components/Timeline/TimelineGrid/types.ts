import { SelectedTimePeriod, SelectTimePeriod, TimelineProps } from '../types';

import { MeetingRoomBooking } from '@/types/booking';

export interface TimelineGridProps
	extends Required<Pick<TimelineProps, 'data' | 'selectedDate'>> {
	selectedPeriod: SelectedTimePeriod<number> | null;
	onPeriodSelect: SelectTimePeriod<number>;
	handleCancelClick?: VoidFunction;
	onSetItemToDelete?: (booking: MeetingRoomBooking | null) => void;
	selectedSectionName?: string | null;
	selectedSectionId?: number | null;
}

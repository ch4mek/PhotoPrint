import { Timezone } from '@/components/Timeline/types';
import { MeetingRoomBooking } from '@/types/booking';

export interface RoomBookingListItemProps extends MeetingRoomBooking {
	userId: number;
	handleDelete: VoidFunction;
	handleClickName: (
		userId: string | number,
		isNeedOpenInfo: boolean
	) => VoidFunction;
	timeZone: Timezone;
	isNameClickable: boolean;
}

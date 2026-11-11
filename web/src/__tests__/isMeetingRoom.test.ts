import { isMeetingRoom } from '@/helpers/isMeetingRoom';
import { DeskBooking, MeetingRoomBooking } from '@/types/booking';

describe('Helper function isMeetingRoom', () => {
	it('should return true for MeetingRoomBooking object', () => {
		const meetingRoom = {
			id: 'room1',
			reservations: [],
		} as unknown as MeetingRoomBooking;

		expect(isMeetingRoom(meetingRoom)).toBe(true);
	});

	it('should return false for DeskBooking object', () => {
		const desk = {} as unknown as DeskBooking;
		expect(isMeetingRoom(desk)).toBe(false);
	});
});

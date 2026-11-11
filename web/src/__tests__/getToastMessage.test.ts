import { TFunction } from 'i18next';

import { createPeriodInText } from '@/helpers/booking';
import { getFormattedDateTimePeriodWithUTC } from '@/helpers/bookingFormatters';
import { getMeetingRoomBookedForPeriod } from '@/helpers/getToastMessage';
import { MeetingRoomsCollision } from '@/queries/booking/types';

jest.mock('@/helpers/booking', () => ({
	createPeriodInText: jest.fn(() => 'mocked period'),
}));

jest.mock('@/helpers/bookingFormatters', () => ({
	getFormattedDateTimePeriodWithUTC: jest.fn(() => 'mocked time period'),
}));

const mockBookingT = ((
	key: string,
	options?: { room?: string; period?: string }
) => {
	if (key === 'warnings.meetingRoomCollision') {
		return `You cannot book it, because the ${options?.room} is booked for the following period: ${options?.period}`;
	}

	return key;
}) as TFunction<'booking'>;

describe('Helper function getToastMessage', () => {
	const mockCollision = {
		reservationFromOfficeDateTime: '2023-12-01T10:00:00Z',
		reservationToOfficeDateTime: '2023-12-01T11:00:00Z',
		meetingRoomName: 1,
	} as MeetingRoomsCollision;

	it('should generate correct booking conflict message', () => {
		const result = getMeetingRoomBookedForPeriod(mockCollision, mockBookingT);

		expect(result).toBe(
			'You cannot book it, because the 1 is booked for the following period: mocked period, mocked time period'
		);
	});

	it('should call helper functions with correct parameters', () => {
		getMeetingRoomBookedForPeriod(mockCollision, mockBookingT);

		expect(createPeriodInText).toHaveBeenCalledWith(0, [
			'2023-12-01T10:00:00Z',
		]);
		expect(getFormattedDateTimePeriodWithUTC).toHaveBeenCalledWith(
			'2023-12-01T10:00:00Z',
			'2023-12-01T11:00:00Z',
			expect.any(Function)
		);
	});

	it('should handle different room names', () => {
		const collisionWithDifferentRoom = {
			...mockCollision,
			meetingRoomName: 2,
		} as unknown as MeetingRoomsCollision;

		const result = getMeetingRoomBookedForPeriod(
			collisionWithDifferentRoom,
			mockBookingT
		);

		expect(result).toContain('2');
	});
});

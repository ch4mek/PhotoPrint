import { BookedDataResponse } from '../types';

import { MeetingRoomType } from '@/types/configuration';

export const createTimelineSkeleton = (
	meetingRoomsConfig: MeetingRoomType[]
): Array<BookedDataResponse> =>
	meetingRoomsConfig.map(({ id, name }) => ({
		meetingRoomId: id,
		meetingRoomName: name,
		reservations: [],
		equipments: [],
		countOfSeats: 0,
	}));

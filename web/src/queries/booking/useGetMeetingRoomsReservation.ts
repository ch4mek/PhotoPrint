import axios from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { BookingQueriesKeys } from '@/constants/queriesKeys';
import { getMeetingRoomsReservationsAPIEndpoint } from '@/constants/queryPaths';
import { useToast } from '@/context/index';
import { createToast } from '@/helpers/createToast';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

import { GetMeetingRoomsReservationsResponse } from './types';

export const useGetMeetingRoomReservations = (
	dayDate: string | null,
	officeId: number
) => {
	const toast = useToast();

	return useQueryWithCallback({
		queryKey: [BookingQueriesKeys.meetingRoomReservations, dayDate],
		queryFn: async () => {
			try {
				if (dayDate) {
					const response = axios.get<GetMeetingRoomsReservationsResponse>(
						getMeetingRoomsReservationsAPIEndpoint(officeId, dayDate)
					);

					return (await response).data;
				}

				return undefined;
			} catch {
				toast.open(createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG));

				return undefined;
			}
		},
		enabled: Boolean(dayDate),
	});
};

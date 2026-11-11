import axios, { AxiosError, isAxiosError } from 'axios';

import { BookingQueriesKeys } from '@/constants/queriesKeys';
import { getMeetingRoomsCollisionsApiUrl } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { GetMeetingRoomsCollisions } from './types';

interface UseCheckMeetingRoomsCollisionsProps {
	meetingRoomId?: number;
	ReservationFromUtc?: string;
	ReservationToUtc?: string;
	ReservationPeriodFromOfficeDate?: string;
	ReservationPeriodToOfficeDate?: string;
	enabled: boolean;
	UserId?: number;
	RepeatReservationWeeks?: number | null;
}

export const useCheckMeetingRoomsCollisions = (
	{
		ReservationFromUtc,
		ReservationToUtc,
		ReservationPeriodFromOfficeDate,
		ReservationPeriodToOfficeDate,
		enabled,
		UserId,
		RepeatReservationWeeks,
		meetingRoomId,
	}: UseCheckMeetingRoomsCollisionsProps,
	options?: PatchedQueryOptions<GetMeetingRoomsCollisions, AxiosError>
) =>
	useQueryWithCallback<GetMeetingRoomsCollisions, AxiosError>({
		queryKey: [
			BookingQueriesKeys.meetingRoomCollisions,
			ReservationFromUtc,
			ReservationToUtc,
			ReservationPeriodFromOfficeDate,
			ReservationPeriodToOfficeDate,
			enabled,
			UserId,
			RepeatReservationWeeks,
			meetingRoomId,
		],
		queryFn: async () => {
			if (meetingRoomId) {
				try {
					const { data } = await axios.get<GetMeetingRoomsCollisions>(
						getMeetingRoomsCollisionsApiUrl(meetingRoomId),
						{
							params: {
								ReservationFromUtc,
								ReservationToUtc,
								ReservationPeriodFromOfficeDate,
								ReservationPeriodToOfficeDate,
								UserId,
								RepeatReservationWeeks,
							},
						}
					);

					return data;
				} catch (error) {
					if (isAxiosError(error)) {
						throw error;
					}
				}
			}

			return [];
		},
		enabled: enabled && Boolean(meetingRoomId),
		refetchOnWindowFocus: false,
		...options,
	});

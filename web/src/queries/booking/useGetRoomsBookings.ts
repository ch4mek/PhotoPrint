import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { BookingQueriesKeys } from '@/constants/queriesKeys';
import { getMeetingRoomsBookingsAPIEndpoint } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { GetRoomsBookingResponse } from './types';

export const useGetRoomsBookings = (
	officeId: number,
	options?: PatchedQueryOptions<GetRoomsBookingResponse, AxiosError>
) =>
	useQueryWithCallback<GetRoomsBookingResponse, AxiosError>({
		queryKey: [BookingQueriesKeys.meetingRoomsBookings, officeId],
		queryFn: async () => {
			try {
				const { data } = await axios.get<GetRoomsBookingResponse>(
					getMeetingRoomsBookingsAPIEndpoint(officeId)
				);

				return data;
			} catch (error) {
				throw error as AxiosError;
			}
		},
		placeholderData: keepPreviousData,
		enabled: Boolean(officeId),
		refetchOnWindowFocus: false,
		...options,
	});

import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { BookingQueriesKeys } from '@/constants/queriesKeys';
import { GET_BOOKING_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { GetBookingResponse } from './types';

interface UseGetBookingsOptions {
	officeId: number;
	userId: number;
	includeEmployee: boolean;
}

export const useGetBookings = (
	{ officeId, userId, includeEmployee }: UseGetBookingsOptions,
	options?: PatchedQueryOptions<GetBookingResponse, AxiosError>
) =>
	useQueryWithCallback<GetBookingResponse, AxiosError>({
		queryKey: [BookingQueriesKeys.bookings, userId, includeEmployee, officeId],
		queryFn: async () => {
			try {
				const { data } = await axios.get<GetBookingResponse>(
					GET_BOOKING_API_ENDPOINT,
					{
						params: {
							userId,
							includeEmployee,
							officeId,
						},
					}
				);

				return data;
			} catch (error) {
				throw error as AxiosError;
			}
		},
		placeholderData: keepPreviousData,
		enabled: Boolean(userId),
		refetchOnWindowFocus: false,
		...options,
	});

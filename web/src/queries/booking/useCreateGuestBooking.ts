import { UseMutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError, isAxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_BOOKING_API_ENDPOINT } from '@/constants/queryPaths';
import { getDateAsResponseType } from '@/helpers/booking';
import { Dayjs } from '@/lib/dayjs';

import { CreateGuestBookingVariables } from './types';
import { getRequestType } from './utils';

export const useCreateGuestBooking = (
	options: UseMutationOptions<
		void,
		AxiosError<ServerResponseError>,
		CreateGuestBookingVariables
	>
) =>
	useMutation<
		void,
		AxiosError<ServerResponseError>,
		CreateGuestBookingVariables
	>({
		...options,
		mutationFn: ({
			requestType,
			workplaceId,
			fromDate,
			toDate,
			weeksInterval,
			officeId,
			guestName,
		}) =>
			axios
				.post(GET_BOOKING_API_ENDPOINT, {
					requestType: getRequestType(requestType, undefined),
					workplaceId,
					fromDate: getDateAsResponseType(fromDate as Dayjs),
					toDate: getDateAsResponseType(toDate as Dayjs),
					weeksInInterval: weeksInterval,
					officeId,
					guestName,
				})
				.then((response) => response.data)
				.catch((error) => {
					if (isAxiosError(error)) {
						throw error;
					}
				}),
	});

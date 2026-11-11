import { UseMutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError, isAxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_BOOKING_API_ENDPOINT } from '@/constants/queryPaths';
import { getDateAsResponseType } from '@/helpers/booking';
import { Dayjs } from '@/lib/dayjs';

import { CreateMutationVariables } from './types';
import { getRequestType } from './utils';

export const useCreateBooking = (
	options: UseMutationOptions<
		void,
		AxiosError<ServerResponseError>,
		CreateMutationVariables
	>
) =>
	useMutation<void, AxiosError<ServerResponseError>, CreateMutationVariables>({
		...options,
		mutationFn: ({
			requestType,
			workplaceId,
			fromDate,
			toDate,
			weeksInterval,
			officeId,
			userId = null,
		}) =>
			axios
				.post(GET_BOOKING_API_ENDPOINT, {
					requestType: getRequestType(requestType, userId),
					workplaceId,
					fromDate: getDateAsResponseType(fromDate as Dayjs),
					toDate: getDateAsResponseType(toDate as Dayjs),
					weeksInInterval: weeksInterval,
					userId,
					officeId,
				})
				.then((response) => response.data)
				.catch((error) => {
					if (isAxiosError(error)) {
						throw error;
					}
				}),
	});

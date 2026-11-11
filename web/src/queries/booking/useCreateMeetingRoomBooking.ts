import { UseMutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { createMeetingRoomBooking } from '@/constants/queryPaths';

import { CreateMeetingRoomBookingVariables } from './types';

export const useCreateMeetingRoomBooking = (
	options?: UseMutationOptions<
		void,
		AxiosError,
		CreateMeetingRoomBookingVariables
	>
) =>
	useMutation<void, AxiosError, CreateMeetingRoomBookingVariables>({
		...options,
		mutationFn: ({ roomId, userId, ...restProps }) =>
			axios
				.post(createMeetingRoomBooking(roomId), {
					userId: userId || undefined,
					...restProps,
				})
				.then((response) => response.data),
	});

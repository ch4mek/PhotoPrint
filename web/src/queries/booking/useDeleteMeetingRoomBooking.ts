import { UseMutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { HttpStatuses } from '@/constants/httpStatuses';
import { NotificationType } from '@/constants/notifications';
import { createMeetingRoomDeleteBookingApiUrl } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';

import { DeleteMeetingRoomVariables } from './types';

export const useDeleteMeetingRoomBooking = (
	options: UseMutationOptions<void, AxiosError, DeleteMeetingRoomVariables>,
	toast: ToastContextValues
) =>
	useMutation<void, AxiosError, DeleteMeetingRoomVariables>({
		...options,
		mutationFn: ({ entryId }) =>
			axios
				.delete(createMeetingRoomDeleteBookingApiUrl(entryId))
				.then(() => {})
				.catch((error: unknown) => {
					if (
						error instanceof AxiosError &&
						error.response?.status !== HttpStatuses.NotFound
					) {
						toast.open(
							createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG)
						);
					}
				}),
	});

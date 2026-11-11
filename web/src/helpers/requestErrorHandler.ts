import { AxiosError, isAxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { ToastContextValues } from '@/context/types';
import { ServerResponseError } from '@/queries/types';

import { createToast } from './createToast';

export const createOnErrorHandler =
	(toast: ToastContextValues, title?: string) =>
	(error: AxiosError<ServerResponseError, unknown>) => {
		const errorMessage =
			error.response?.data?.Message || error.response?.data?.message || '';

		toast.open(
			createToast(
				NotificationType.ERROR,
				title
					? `${title}: ${errorMessage}`
					: errorMessage || SOMETHING_WENT_WRONG
			)
		);
	};

export const handleRequestError = (error: unknown) => {
	if (isAxiosError(error)) {
		throw error;
	}
	throw new Error(SOMETHING_WENT_WRONG);
};

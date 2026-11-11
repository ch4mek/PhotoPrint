import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { BookingQueriesKeys } from '@/constants/queriesKeys';
import { GET_RESERVATION_API_ENDPOINT } from '@/constants/queryPaths';
import { useToast } from '@/context/index';
import { createToast } from '@/helpers/createToast';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

import { GetReservationResponse } from './types';

interface UseGetReservationOptions {
	fromDate: string | undefined | null;
	toDate: string | undefined | null;
	officeId: number;
	error: string | null;
	workspaceId: number | undefined;
	enabled: boolean;
}

export const useGetReservation = ({
	fromDate,
	toDate,
	officeId,
	error,
	workspaceId,
	enabled,
}: UseGetReservationOptions) => {
	const toast = useToast();

	return useQueryWithCallback({
		queryKey: [
			BookingQueriesKeys.reservations,
			fromDate,
			toDate,
			officeId,
			workspaceId,
			enabled,
		],
		queryFn: async () => {
			try {
				if (officeId && fromDate && toDate && !error) {
					const response = axios.get<GetReservationResponse>(
						GET_RESERVATION_API_ENDPOINT,
						{
							params: {
								fromDate: fromDate.replace(/\+.*/, 'Z'),
								toDate: toDate.replace(/\+.*/, 'Z'),
								officeId,
								workspaceId,
							},
						}
					);

					return (await response).data;
				}

				return undefined;
			} catch {
				toast.open(createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG));

				return undefined;
			}
		},
		placeholderData: keepPreviousData,
		enabled: enabled && Boolean(fromDate) && Boolean(toDate),
		refetchOnWindowFocus: false,
	});
};

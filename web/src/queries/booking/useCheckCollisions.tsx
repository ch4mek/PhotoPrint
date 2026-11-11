import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { NotificationType } from '@/constants/notifications';
import { BookingQueriesKeys } from '@/constants/queriesKeys';
import { GET_COLLISIONS_API_ENDPOINT } from '@/constants/queryPaths';
import { useToast } from '@/context/index';
import { SelectedDesk } from '@/context/types';
import {
	getDateAsResponseType,
	getTextFromMultipleDate,
} from '@/helpers/booking';
import { createToast } from '@/helpers/createToast';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { Dayjs } from '@/lib/dayjs';
import {
	generateWarningBlockMessage,
	generateWarningBookMessage,
} from '@/pages/Booking/utils/generateWarningMessage';

interface UseCheckCollisionsOptions {
	desk: SelectedDesk | null;
	fromDate: Dayjs | null;
	toDate: Dayjs | null;
	weeksInterval: number;
	officeId: number;
	userId: number;
	isBlockedDesk: boolean;
	onDeskCollisionExist?: () => void;
	onDeskCollisionAbsent?: () => void;
}

export const useCheckDeskCollisions = ({
	desk,
	fromDate,
	toDate,
	weeksInterval,
	officeId,
	userId,
	isBlockedDesk,
	onDeskCollisionAbsent,
	onDeskCollisionExist,
}: UseCheckCollisionsOptions) => {
	// Prevent check for collisions on office change
	const [prevOfficeId, setPrevOfficeId] = useState(officeId);
	const toast = useToast();
	const { t } = useTranslation('booking');

	useEffect(() => {
		setPrevOfficeId(officeId);
	}, [officeId]);

	const handleDeskCollisionExist = () => {
		if (typeof onDeskCollisionExist === 'function') {
			onDeskCollisionExist();
		}
	};

	const handleDeskCollisionAbsent = () => {
		if (typeof onDeskCollisionAbsent === 'function') {
			onDeskCollisionAbsent();
		}
	};

	return useQueryWithCallback({
		queryKey: [
			BookingQueriesKeys.collisions,
			desk?.id,
			fromDate,
			toDate,
			weeksInterval,
			officeId,
			userId,
		],
		queryFn: async () => {
			if (desk && desk.id > 0 && prevOfficeId === officeId) {
				try {
					const response = await axios.get(GET_COLLISIONS_API_ENDPOINT, {
						params: {
							officeId,
							workplaceId: desk.id,
							fromDate: getDateAsResponseType(fromDate as Dayjs),
							toDate: getDateAsResponseType(toDate as Dayjs),
							weeksInterval,
							userId,
						},
					});

					const seatName = `${desk.abbreviation} - ${desk?.displayNumber}`;

					if (response.data.length > 0) {
						if (!desk.isCanCanceled) {
							const message = isBlockedDesk
								? generateWarningBlockMessage(seatName, t)
								: generateWarningBookMessage(
										seatName,
										getTextFromMultipleDate(response.data),
										t
								  );

							toast.open(createToast(NotificationType.INFO, message));
						}

						handleDeskCollisionExist();

						return true;
					}
				} catch {
					handleDeskCollisionExist();

					return true;
				}

				handleDeskCollisionAbsent();

				return false;
			}

			handleDeskCollisionAbsent();

			return false;
		},
		placeholderData: keepPreviousData,
		enabled: Boolean(desk?.id) && Boolean(fromDate) && Boolean(toDate),
		refetchOnWindowFocus: false,
	});
};

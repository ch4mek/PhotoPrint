import { TFunction } from 'i18next';

import { getNearestQuarterHourTime } from '@/helpers/bookingTime';
import { Dayjs } from '@/lib/dayjs';

export const getAvailabilityMessage = (
	isAvailable: boolean,
	t: TFunction<'booking', 'form.roomAvailability'>
) => (isAvailable ? t('available') : t('unavailable'));

export const getNearestTimeSelectorOption = (
	date?: Dayjs,
	timeStepOffset = 0
) => {
	const nearestTime = getNearestQuarterHourTime(date, timeStepOffset);

	return {
		id: nearestTime,
		name: nearestTime,
	};
};

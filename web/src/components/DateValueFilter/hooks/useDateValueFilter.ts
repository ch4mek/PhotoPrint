import { useCallback, useState } from 'react';

import { getCurrentMonthDate } from '@/helpers/monthYearDate';
import { Dayjs } from '@/lib/dayjs';

export function useDateValueFilter() {
	const [date, setDate] = useState(getCurrentMonthDate);

	const handleSetDate = useCallback((value: Dayjs) => {
		setDate(value.startOf('month'));
	}, []);

	const resetDate = useCallback(() => {
		setDate(getCurrentMonthDate());
	}, []);

	const isResetDisabled = date.isSame(getCurrentMonthDate(), 'month');

	return {
		date,
		setDate: handleSetDate,
		resetDate,
		isResetDisabled,
	};
}

import { useCallback, useState } from 'react';

import { CURRENT_YEAR } from '@/constants/date';

const CURRENT_YEAR_STRING = CURRENT_YEAR.toString();

export function useYearValueFilter() {
	const [year, setYear] = useState(CURRENT_YEAR_STRING);

	const resetYear = useCallback(() => {
		setYear(CURRENT_YEAR_STRING);
	}, []);

	const isResetDisabled = year === CURRENT_YEAR_STRING;

	return {
		year,
		setYear,
		resetYear,
		isResetDisabled,
	};
}

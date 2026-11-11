import { useTheme } from '@emotion/react';
import { useCallback, useMemo } from 'react';

import { FilterAutoselect } from '../FilterAutoselect';

import { dateNavigatorSelectors } from '@/constants/date';
import { checkIfPrimitiveOptionsEqual } from '@/helpers/checkIfOptionEqualToValue';
import {
	changeSelectDate,
	generateDateOptions,
	generateShorthandOfQuarter,
} from '@/helpers/date';
import { generateInitialData } from '@/helpers/generateInitialData';
import { TimePeriod } from '@/types/index';

import type { DateSelectorProps } from './types';

export function DateSelector({
	date,
	timePeriod,
	setDate,
	showToast,
	setTimePeriod,
	shouldBeClosedOnOutsideScroll,
	isMobileView = false,
}: DateSelectorProps) {
	const timePeriodValue = useMemo(() => {
		const { year, month, quarter } = date;
		const timeMonth = month ? `${month}, ` : '';
		const timeYear = year;

		return `${generateShorthandOfQuarter(quarter)}${timeMonth}${timeYear}`;
	}, [date, timePeriod]);

	const dateOptions = useMemo(
		() => generateDateOptions(timePeriod),
		[timePeriod]
	);

	const handleChange = useCallback((value: TimePeriod | null) => {
		if (!value) {
			return;
		}
		showToast();
		setTimePeriod(value);
		setDate(generateInitialData(value));
	}, []);

	const handleChooseDate = useCallback(
		(value: string | null) => {
			if (!value) {
				return;
			}
			showToast();
			setDate(changeSelectDate(timePeriod, value));
		},
		[date, timePeriod]
	);

	const filterOptions = useMemo(
		() => dateNavigatorSelectors.map(({ title }) => title) as TimePeriod[],
		[dateNavigatorSelectors]
	);

	const theme = useTheme();

	const stylesConfig = {
		backgroundColor: theme.palette['background-white'],
		popover: {
			position: 'fixed',
		},
	} as const;

	return (
		<>
			<FilterAutoselect
				value={timePeriod}
				options={filterOptions}
				onChange={handleChange}
				valueToClean={TimePeriod.Month}
				stylesConfig={stylesConfig}
				label=""
				checkIfOptionEqualToValue={checkIfPrimitiveOptionsEqual}
				shouldBeClosedOnOutsideScroll={shouldBeClosedOnOutsideScroll}
				isMobileView={isMobileView}
			/>
			<FilterAutoselect
				options={dateOptions}
				value={timePeriodValue}
				onChange={handleChooseDate}
				label=""
				valueToClean={dateOptions[0]}
				stylesConfig={stylesConfig}
				checkIfOptionEqualToValue={checkIfPrimitiveOptionsEqual}
				shouldBeClosedOnOutsideScroll={shouldBeClosedOnOutsideScroll}
				isMobileView={isMobileView}
			/>
		</>
	);
}

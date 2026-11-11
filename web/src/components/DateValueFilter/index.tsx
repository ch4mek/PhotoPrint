import { useTheme } from '@emotion/react';
import { useCallback, useMemo } from 'react';

import { FilterAutoselect } from '../FilterAutoselect';

import { checkIfPrimitiveOptionsEqual } from '@/helpers/checkIfOptionEqualToValue';
import {
	formatMonthYearDate,
	getMonthYearOptions,
	parseMonthYearOption,
} from '@/helpers/monthYearDate';
import { MonthYearOption } from '@/types/forms';

import type { DateValueFilterProps } from './types';

export function DateValueFilter({
	date,
	onDateChange,
	onChange,
	shouldBeClosedOnOutsideScroll,
}: DateValueFilterProps) {
	const theme = useTheme();

	const selectedValue = useMemo(() => formatMonthYearDate(date), [date]);
	const options = useMemo(() => getMonthYearOptions(), []);

	const stylesConfig = {
		backgroundColor: theme.palette['background-white'],
		popover: {
			position: 'fixed',
		},
	} as const;

	const handleChange = useCallback(
		(value: MonthYearOption | null) => {
			if (!value) {
				return;
			}

			onChange?.();
			onDateChange(parseMonthYearOption(value));
		},
		[onChange, onDateChange]
	);

	return (
		<FilterAutoselect
			value={selectedValue}
			onChange={handleChange}
			label=""
			valueToClean={options[0]}
			checkIfOptionEqualToValue={checkIfPrimitiveOptionsEqual}
			{...{ stylesConfig, options, shouldBeClosedOnOutsideScroll }}
		/>
	);
}

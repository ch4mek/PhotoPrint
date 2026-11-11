import { useMemo } from 'react';

import { getCurrentMonday } from '../components/TimesheetPageControls/utils';

import { DataTypes } from '@/constants/dataTypes';
import { filterByPeriod } from '@/constants/filters';
import {
	TS_DATE_FILTER_STORAGE_KEY,
	TS_PERIOD_FILTER_STORAGE_KEY,
	TS_PERSONAL_MODE_DATE_FILTER_STORAGE_KEY,
	TS_PERSONAL_MODE_PERIOD_FILTER_STORAGE_KEY,
} from '@/constants/storageKeys';
import { useStickyState } from '@/hooks/useStickyState';

export function useDateFilters(isPersonalModeOn: boolean) {
	const [defaultDateFilter, setDefaultDateFilter] = useStickyState(
		getCurrentMonday(),
		TS_DATE_FILTER_STORAGE_KEY,
		DataTypes.Date
	);
	const [personalModeDateFilter, setPersonalModeDateFilter] = useStickyState(
		getCurrentMonday(),
		TS_PERSONAL_MODE_DATE_FILTER_STORAGE_KEY,
		DataTypes.Date
	);

	const [defaultPeriodFilter, setDefaultPeriodFilter] = useStickyState(
		filterByPeriod.weekFilter,
		TS_PERIOD_FILTER_STORAGE_KEY
	);
	const [personalModePeriodFilter, setPersonalModePeriodFilter] =
		useStickyState(
			filterByPeriod.weekFilter,
			TS_PERSONAL_MODE_PERIOD_FILTER_STORAGE_KEY
		);

	const resetDateFilters = () => {
		setDefaultPeriodFilter(filterByPeriod.weekFilter);
		setPersonalModePeriodFilter(filterByPeriod.weekFilter);
		setDefaultDateFilter(getCurrentMonday());
		setPersonalModeDateFilter(getCurrentMonday());
	};

	const returnValue = useMemo(() => {
		if (isPersonalModeOn) {
			return {
				date: personalModeDateFilter,
				setDate: setPersonalModeDateFilter,
				periodFilter: personalModePeriodFilter,
				setPeriodFilter: setPersonalModePeriodFilter,
				resetDateFilters,
			};
		}

		return {
			date: defaultDateFilter,
			setDate: setDefaultDateFilter,
			periodFilter: defaultPeriodFilter,
			setPeriodFilter: setDefaultPeriodFilter,
			resetDateFilters,
		};
	}, [
		isPersonalModeOn,
		personalModeDateFilter,
		defaultDateFilter,
		personalModePeriodFilter,
		defaultPeriodFilter,
	]);

	return returnValue;
}

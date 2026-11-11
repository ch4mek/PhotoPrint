import { useCallback, useMemo } from 'react';

import { initialInquiryFilterValue } from '../constants';

import { ALL_ID } from '@/constants/filters';
import { checkIsDocumentRequest } from '@/helpers/absences';
import { generateAvailableRangeFromCompanyFoundation } from '@/helpers/date';
import { INITIAL_SELECTED_DATE } from '@/pages/Requests/components/CalendarTab/constants';
import { useCalendarFilters } from '@/pages/Requests/components/CalendarTab/context/CalendarFiltersContext';
import { MonthYearDate } from '@/pages/Requests/components/CalendarTab/types';
import {
	formatMonthYearToString,
	getNextMonth,
	getNextYearDate,
	getPrevMonth,
	parseStringToMonthYear,
} from '@/pages/Requests/components/CalendarTab/utils';
import { useInquiriesLookup } from '@/queries/inquiries/useInquiriesLookup';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';
import { RangeTimePeriod } from '@/types/index';

export const useCalendarFiltersController = () => {
	const {
		inquiryTemplateName,
		setInquiryTemplateName,
		setInquiryId,
		date,
		setDate,
		resetFilters,
	} = useCalendarFilters();

	const dateOptions = useMemo(
		() =>
			generateAvailableRangeFromCompanyFoundation(
				RangeTimePeriod.Month,
				getNextYearDate()
			) as MonthYearDate[],
		[]
	);

	const selectedDateOption = useMemo(
		() => formatMonthYearToString(date),
		[date]
	);

	const handleDateChange = useCallback((value: MonthYearDate | null) => {
		if (!value) {
			return;
		}

		setDate(parseStringToMonthYear(value));
	}, []);

	const handlePrevMonth = useCallback(() => {
		setDate(getPrevMonth(date));
	}, [date]);

	const handleNextMonth = useCallback(() => {
		setDate(getNextMonth(date));
	}, [date]);

	const { data: inquiries = [] } = useInquiriesLookup();

	const inquiryOptions: SimpleConfigurableConfigurationItem[] = useMemo(
		() => [
			initialInquiryFilterValue,
			...inquiries
				.filter(({ inquiryType }) => !checkIsDocumentRequest(inquiryType))
				.map(({ name, id }) => ({ id: String(id), name })),
		],
		[inquiries]
	);

	const selectedInquiryOption = useMemo(() => {
		if (inquiryTemplateName === null) {
			return initialInquiryFilterValue;
		}

		return (
			inquiryOptions.find((item) => item.name === inquiryTemplateName) ||
			initialInquiryFilterValue
		);
	}, [inquiryTemplateName, inquiryOptions]);

	const handleInquiryChange = useCallback(
		(item: SimpleConfigurableConfigurationItem | null) => {
			if (!item || item.id === ALL_ID) {
				setInquiryTemplateName(null);
				setInquiryId(null);

				return;
			}
			setInquiryTemplateName(item.name);
			setInquiryId(item.id);
		},
		[]
	);

	const firstDate = useMemo(
		() => parseStringToMonthYear(dateOptions[0]),
		[dateOptions]
	);
	const lastDate = useMemo(
		() => parseStringToMonthYear(dateOptions[dateOptions.length - 1]),
		[dateOptions]
	);

	const isNextArrowDisabled =
		date.month === firstDate.month && date.year === firstDate.year;

	const isPrevArrowDisabled =
		date.month === lastDate.month && date.year === lastDate.year;

	const isResetDisabled = useMemo(
		() =>
			inquiryTemplateName === null &&
			date.month === INITIAL_SELECTED_DATE.month &&
			date.year === INITIAL_SELECTED_DATE.year,
		[inquiryTemplateName, date]
	);

	return {
		inquiryOptions,
		selectedInquiryOption,
		handleInquiryChange,
		resetFilters,
		dateOptions,
		selectedDateOption,
		handleDateChange,
		handlePrevMonth,
		handleNextMonth,
		isPrevArrowDisabled,
		isNextArrowDisabled,
		isResetDisabled,
	};
};

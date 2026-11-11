import { useTheme } from '@emotion/react';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { BookingCalendarIcon } from '@mms/mms-ui-library/icons/base/BookingCalendarIcon';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { useBookingContext } from '@/context/index';
import { getOfficeBookingRange } from '@/helpers/bookingTime';
import dayjs, { Dayjs } from '@/lib/dayjs';

import { DatePickerError } from './constants';
import {
	DatePickersContentWrapper,
	DatePickersHeader,
	DatePickersWrapper,
} from './styled';
import { getDatePickerErrorMessage, getParseInputDateToDate } from './utils';

export function DatePickers() {
	const {
		firstDay,
		setFirstDay,
		lastDay,
		setLastDay,
		setError,
		error,
		timelineDate,
		areMeetingRoomsSelected,
		setTimelineDate,
		office: { timeZone },
	} = useBookingContext();
	const [disabled, setDisabled] = useState(false);
	const theme = useTheme();
	const { t } = useTranslation('booking');
	const { t: tCommon } = useTranslation('common', {
		keyPrefix: 'placeholders',
	});

	const setTimelineDateIfNeeded = useCallback(
		(dayjsValue: Dayjs) => {
			if (areMeetingRoomsSelected) {
				const dayjsValueWithTimezone = dayjsValue.clone().tz(timeZone, true);

				if (!dayjsValueWithTimezone.isSame(timelineDate, 'day')) {
					setTimelineDate(dayjsValueWithTimezone);
				}
			}
		},
		[areMeetingRoomsSelected, timelineDate, timeZone]
	);

	const { currentOfficeDateInThreeMonth, currentOfficeDate: minDate } = useMemo(
		() => getOfficeBookingRange(timeZone),
		[timeZone]
	);

	const handleChangeFirstDay = useCallback(
		(newValue: Dayjs | null) => {
			if (!newValue) {
				setFirstDay(null);
				setLastDay(null);
				setDisabled(true);
				setError(DatePickerError.InvalidDate);

				return;
			}

			const currentOfficeDate = dayjs().tz(timeZone, true).startOf('day');

			const dayjsValue = dayjs(newValue).tz(timeZone, true).startOf('day');

			if (dayjsValue?.isBefore(currentOfficeDate.clone(), 'day')) {
				setTimelineDateIfNeeded(dayjsValue);
				setError(DatePickerError.MinDate);

				return;
			}

			if (dayjsValue?.isAfter(currentOfficeDateInThreeMonth, 'day')) {
				setTimelineDateIfNeeded(dayjsValue);
				setError(DatePickerError.MaxDate);

				return;
			}

			if (!dayjsValue?.isValid()) {
				setError(DatePickerError.PastDate);
				setDisabled(true);

				return;
			}

			if (dayjsValue.isBefore(currentOfficeDate, 'day')) {
				setError(DatePickerError.PastDate);

				return;
			}

			setDisabled(false);
			setError(null);
			setFirstDay(dayjsValue);

			setTimelineDateIfNeeded(dayjsValue);

			if (!dayjsValue.isSame(firstDay)) {
				setLastDay(dayjsValue);
			}
		},
		[disabled, firstDay, setTimelineDateIfNeeded, timeZone]
	);

	const handleChangeLastDay = useCallback(
		(newValue: Dayjs | null) => {
			if (!newValue) {
				setLastDay(null);
				setError(DatePickerError.InvalidDate);

				return;
			}

			const currentOfficeDate = dayjs().tz(timeZone, true).startOf('day');

			const dayjsValue = dayjs(newValue).tz(timeZone, true).startOf('day');

			if (dayjsValue?.isBefore(firstDay, 'day')) {
				setError(DatePickerError.MinDate);

				return;
			}

			if (dayjsValue?.isAfter(currentOfficeDateInThreeMonth, 'day')) {
				setError(DatePickerError.MaxDate);

				return;
			}

			if (dayjsValue?.isBefore(currentOfficeDate, 'day')) {
				setError(DatePickerError.PastDate);

				return;
			}

			if (firstDay?.isBefore(currentOfficeDate, 'day')) {
				setError(DatePickerError.PastDate);

				return;
			}
			setError(null);
			setLastDay(dayjsValue);
		},
		[firstDay, lastDay, setError, setLastDay]
	);

	useEffect(() => {
		const startOfCurrentDate = dayjs().tz(timeZone, true).startOf('day');

		const isDateValid =
			lastDay?.isSameOrAfter(firstDay) &&
			firstDay?.isSameOrAfter(startOfCurrentDate);

		if (isDateValid) {
			setError(null);
			setDisabled(false);
		}
	}, [firstDay, lastDay, timeZone]);

	const datePickerStylesConfig = useMemo(
		() => ({
			label: {
				background: theme.palette['background-white'],
			},
			errorText: {
				wrapper: {
					background: theme.palette['background-white'],
				},
			},
		}),
		[theme]
	);

	const errorMessage = useMemo(() => getDatePickerErrorMessage(error), [error]);

	const commonDatePickerProps = (pickerMinDate: Dayjs | undefined) => ({
		calendarProps: {
			timezone: timeZone,
			localizationProviderProps: {
				dateAdapter: AdapterDayjs,
				dateLibInstance: dayjs,
			},
			minDate: pickerMinDate,
			maxDate: currentOfficeDateInThreeMonth,
		},
		calendarPosition: 'bottom-start' as const,
		stylesConfig: datePickerStylesConfig,
		errorText: errorMessage,
		parseInputDateToDate: getParseInputDateToDate(timeZone),
	});

	return (
		<DatePickersWrapper>
			<DatePickersHeader>
				<BookingCalendarIcon />
				<Typography variant="m-600">{t('form.dateLabel')}</Typography>
			</DatePickersHeader>
			<DatePickersContentWrapper>
				<DatePicker
					innerLabel={tCommon('from')}
					inputId="date-picker-from"
					value={firstDay}
					size="fullWidth"
					onChange={handleChangeFirstDay}
					{...commonDatePickerProps(minDate)}
				/>
				<DatePicker
					innerLabel={tCommon('to')}
					inputId="date-picker-to"
					value={lastDay}
					disabled={disabled}
					onChange={handleChangeLastDay}
					size="fullWidth"
					{...commonDatePickerProps(firstDay ?? undefined)}
				/>
			</DatePickersContentWrapper>
		</DatePickersWrapper>
	);
}

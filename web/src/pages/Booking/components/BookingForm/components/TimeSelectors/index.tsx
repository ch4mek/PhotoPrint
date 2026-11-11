import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { ClockIcon } from '@mui/x-date-pickers';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import {
	TIME_SELECTOR_STEP_IN_MINUTES,
	VIEW_TIME_FORMAT,
} from '@/constants/booking';
import { useBookingContext } from '@/context/Booking';
import { getFormattedTime } from '@/helpers/bookingFormatters';
import {
	calculateTimeDifferenceInMinutes,
	generateFromTimeOptions,
	generateToTimeOptions,
	getNearestQuarterHourTime,
} from '@/helpers/bookingTime';
import { minsToHrsAndMinsObject } from '@/helpers/translateTimeUtils';
import dayjs from '@/lib/dayjs';
import { AutoselectOption } from '@/types/common';

import {
	AvailabilityTag,
	IconWrapper,
	TimeRangeDifferenceContainer,
	TimeRangeDifferenceTypography,
	TimeSelectorsContentWrapper,
	TimeSelectorsHeader,
	TimeSelectorsWrapper,
} from './styled';
import { getAvailabilityMessage, getNearestTimeSelectorOption } from './utils';

const ZEROS_TIME_OPTION = {
	id: getFormattedTime(0, 0),
	name: getFormattedTime(0, 0),
};

export function TimeSeletors() {
	const {
		setSelectedTime,
		isWorkingHours,
		isAllDayChecked,
		selectedTime,
		selectedItem,
		isTimelineInRange,
		isSelectedRoomAvailable,
		isSelectedMeetingRoomTimeInPast,
		areMeetingRoomsCollisionsLoading,
		office,
	} = useBookingContext();
	const { t } = useTranslation('booking');
	const { t: tRoomAvailability } = useTranslation('booking', {
		keyPrefix: 'form.roomAvailability',
	});

	const { t: tCommon } = useTranslation('common');

	const theme = useTheme();

	const areSelectorsDisabled = isAllDayChecked || selectedItem === null;

	const fromTimeOption = selectedTime
		? getNearestTimeSelectorOption(selectedTime.from)
		: getNearestTimeSelectorOption(undefined);

	const toTimeOption = selectedTime
		? getNearestTimeSelectorOption(selectedTime.to)
		: getNearestTimeSelectorOption(undefined, 1);

	const isAvailable =
		isSelectedRoomAvailable && !isSelectedMeetingRoomTimeInPast;

	const { hours: differenceHours, minutes: differenceMinutes } = useMemo(
		() =>
			minsToHrsAndMinsObject(
				calculateTimeDifferenceInMinutes(
					selectedTime?.from,
					selectedTime?.to,
					office.timeZone
				)
			),
		[selectedTime, office]
	);

	const fromOptions = useMemo(
		() => generateFromTimeOptions(isWorkingHours),
		[isWorkingHours]
	);

	const toOptions = useMemo(
		() => generateToTimeOptions(isWorkingHours),
		[isWorkingHours]
	);

	const handleFromTimeChange = (option: AutoselectOption<string> | null) => {
		if (option) {
			const newFromTime = dayjs(option.name, VIEW_TIME_FORMAT);
			const newSelectedTime = {
				from: newFromTime,
				to:
					selectedTime?.to ||
					dayjs(getNearestQuarterHourTime(newFromTime, 1), VIEW_TIME_FORMAT),
			};

			if (
				toTimeOption.id !== ZEROS_TIME_OPTION.id &&
				option.id >= toTimeOption.id
			) {
				newSelectedTime.to = newSelectedTime.from
					.clone()
					.add(TIME_SELECTOR_STEP_IN_MINUTES, 'minutes');
			}

			setSelectedTime(newSelectedTime);
		}
	};

	const displayAvailabilityMessage =
		selectedItem && isTimelineInRange && !areMeetingRoomsCollisionsLoading;

	const handleToTimeChange = (option: AutoselectOption<string> | null) => {
		if (option) {
			const newToTime = dayjs(option.name, VIEW_TIME_FORMAT);
			const newSelectedTime = {
				to: newToTime,
				from:
					selectedTime?.from ||
					dayjs(getNearestQuarterHourTime(newToTime, -1), VIEW_TIME_FORMAT),
			};

			if (option.id === ZEROS_TIME_OPTION.id) {
				newSelectedTime.to = newSelectedTime.to.clone().add(1, 'day');
			}

			if (
				option.id !== ZEROS_TIME_OPTION.id &&
				option.id <= fromTimeOption.id
			) {
				newSelectedTime.from = newSelectedTime.to
					.clone()
					.subtract(TIME_SELECTOR_STEP_IN_MINUTES, 'minutes');
			}
			setSelectedTime(newSelectedTime);
		}
	};

	return (
		<TimeSelectorsWrapper>
			<TimeSelectorsHeader>
				<IconWrapper>
					<ClockIcon />
				</IconWrapper>
				<Typography variant="m-600">{t('form.timeLabel')}</Typography>
				{displayAvailabilityMessage && (
					<AvailabilityTag isAvailable={isAvailable}>
						{getAvailabilityMessage(isAvailable, tRoomAvailability)}
					</AvailabilityTag>
				)}
			</TimeSelectorsHeader>
			<TimeSelectorsContentWrapper>
				<Autoselect
					disabled={areSelectorsDisabled}
					value={fromTimeOption}
					options={fromOptions}
					label={tCommon('placeholders.from')}
					valueField="name"
					onChange={handleFromTimeChange}
					disableClearable
					labelAndErrorBgColor={theme.palette['background-white']}
				/>
				<Autoselect
					disabled={areSelectorsDisabled}
					value={toTimeOption}
					options={toOptions}
					label={tCommon('placeholders.to')}
					valueField="name"
					onChange={handleToTimeChange}
					disableClearable
					labelAndErrorBgColor={theme.palette['background-white']}
				/>
				<TimeRangeDifferenceContainer>
					<TimeRangeDifferenceTypography
						disabled={areSelectorsDisabled}
						variant="s-400"
					>
						{Boolean(differenceHours) &&
							`${differenceHours} ${tCommon('time.hours')}`}
						{Boolean(differenceHours) && <br />}
						{Boolean(differenceMinutes) &&
							`${differenceMinutes} ${tCommon('time.minutes')}`}
					</TimeRangeDifferenceTypography>
				</TimeRangeDifferenceContainer>
			</TimeSelectorsContentWrapper>
		</TimeSelectorsWrapper>
	);
}

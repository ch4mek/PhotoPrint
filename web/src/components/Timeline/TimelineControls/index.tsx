import { useTheme } from '@emotion/react';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { Button } from '@mms/mms-ui-library/components/Button';
import { Switch } from '@mms/mms-ui-library/components/Switch';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { ChevronLeftIcon } from '@mms/mms-ui-library/icons/base/ChevronLeftIcon';
import { ChevronRightIcon } from '@mms/mms-ui-library/icons/base/ChevronRightIcon';
import i18next from 'i18next';
import { useTranslation } from 'react-i18next';

import { MobileSwitchWrapper, Row } from '../styles';

import { CONTROLS_BREAKPOINT } from '@/components/Timeline/TimelineControls/constants';

import { useIsMobileView } from './hooks/useIsMobileView';
import { MobileTimeTypography, TimelineControlsWrapper } from './styles';
import { TimelineControlsProps } from './types';
import { getTimezoneLabel } from './utils';

export function TimelineControls({
	isWorkingHoursOnly,
	onWorkingHoursChange,
	selectedDate,
	timezone,
	onGoBackwardDate,
	onGoForwardDate,
	onGoTodayDate,
	isSelectedDayToday,
}: TimelineControlsProps) {
	const { palette } = useTheme();
	const { t } = useTranslation('booking', { keyPrefix: 'timeline' });

	const { timezoneDisplayName: timezoneLocation, timezoneOffset } = timezone;

	const formattedTime = selectedDate.toLocaleDateString(i18next.language, {
		day: '2-digit',
		month: 'short',
		year: 'numeric',
	});

	const { isMobile, containerRef } = useIsMobileView(CONTROLS_BREAKPOINT);
	const switchLabelText = t('workingHours');

	if (isMobile) {
		const showTodayButton = !isSelectedDayToday;

		return (
			<TimelineControlsWrapper isMobileView ref={containerRef}>
				<MobileSwitchWrapper>
					<Row spacing="s">
						<Switch
							checked={isWorkingHoursOnly}
							onChange={onWorkingHoursChange}
						/>
						<Typography variant="m-400" color={palette['text-gray']}>
							{switchLabelText}
						</Typography>
					</Row>
					<Typography color={palette['graphics-gray']} variant="s-400">
						{getTimezoneLabel(timezoneOffset, timezoneLocation, t)}
					</Typography>
				</MobileSwitchWrapper>
				<ActionButton onClick={onGoBackwardDate}>
					<ChevronLeftIcon />
				</ActionButton>
				<MobileTimeTypography variant="m-400" color={palette['text-black']}>
					{formattedTime}
				</MobileTimeTypography>
				{showTodayButton && (
					<Button isFullWidth={false} onClick={onGoTodayDate} color="secondary">
						{t('today')}
					</Button>
				)}
				<ActionButton onClick={onGoForwardDate}>
					<ChevronRightIcon />
				</ActionButton>
			</TimelineControlsWrapper>
		);
	}

	return (
		<TimelineControlsWrapper ref={containerRef}>
			<Row spacing="m">
				<Row spacing="s">
					<Switch
						checked={isWorkingHoursOnly}
						onChange={onWorkingHoursChange}
					/>
					<Typography variant="m-400" color={palette['text-gray']}>
						{switchLabelText}
					</Typography>
				</Row>
				<Button
					disabled={isSelectedDayToday}
					isFullWidth={false}
					onClick={onGoTodayDate}
					color="secondary"
				>
					{t('today')}
				</Button>
				<Row spacing="l">
					<ActionButton onClick={onGoBackwardDate}>
						<ChevronLeftIcon />
					</ActionButton>
					<ActionButton onClick={onGoForwardDate}>
						<ChevronRightIcon />
					</ActionButton>
				</Row>
				<Typography variant="m-400" color={palette['text-black']}>
					{formattedTime}
				</Typography>
			</Row>
			<Typography color={palette['graphics-gray']} variant="s-400">
				{getTimezoneLabel(timezoneOffset, timezoneLocation, t)}
			</Typography>
		</TimelineControlsWrapper>
	);
}

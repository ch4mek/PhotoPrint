import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { PickersDay } from '@mui/x-date-pickers/PickersDay';
import { ComponentType } from 'react';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { DEFAULT_OPACITY } from '@/constants/opacities';
import { zIndexes } from '@/constants/zIndexes';
import { checkIfForwardProps } from '@/helpers/checkIfForwardProps';
import { DAY_CELL_SIZE } from '@/pages/Booking/components/BookingForm/components/BookingCalendar/constants';
import { CUSTOM_PICKER_DAY_PROPS_TO_CHECK } from '@/pages/Booking/components/BookingForm/components/BookingCalendar/Day/constants';

import type { CustomPickersDayProps } from './types';

const DAY_TRANSITION =
	'background-color 1s cubic-bezier(0.04, 0.67, 0.01, 1.01)';

const FIRST_RANGE_RIGHT_SPACE = -2;
const LAST_RANGE_RIGHT_SPACE = 22;
const WIDTH_OF_RANGE_ELEMENT = 16;

const getPseudoElementStyles = (theme: Theme, width: number, right: number) =>
	css({
		content: '" "',
		width,
		height: DAY_CELL_SIZE,
		right,
		top: 0,
		position: 'absolute',
		backgroundColor: theme.palette['background-base-3'],
		zIndex: zIndexes.hidden,
	});

export const CustomPickersDay = styled(PickersDay, {
	shouldForwardProp: checkIfForwardProps(CUSTOM_PICKER_DAY_PROPS_TO_CHECK),
})<CustomPickersDayProps>(
	({
		theme,
		dayIsBetween,
		isFirstDay,
		isLastDay,
		isSameDate,
		completeRange,
		daysIsHoveredBetween,
		isFirstHoveredDay,
		isLastHoveredDay,
		isDayEqualsLastDay,
		isDayEqualsFirstDay,
		isFirstDayOfWeek,
		isLastDayOfWeek,
	}) => ({
		transition: DAY_TRANSITION,
		...((dayIsBetween || daysIsHoveredBetween) && {
			position: 'relative',

			...(!isFirstDay &&
				!isLastDay && {
					'&.MuiPickersDay-root': {
						padding: `0 ${theme.spaces.m}px`,
						borderRadius: BorderRadiuses.none,
						zIndex: 1,
						backgroundColor: theme.palette['background-base-3'],
						opacity: DEFAULT_OPACITY,
					},
				}),

			...(isLastDayOfWeek &&
				!isSameDate &&
				!isDayEqualsLastDay && {
					'&:after': {
						...getPseudoElementStyles(theme, theme.spaces.s, -theme.spaces.s),
					},
				}),

			...(isFirstDayOfWeek &&
				!isSameDate &&
				!isDayEqualsFirstDay && {
					'&:before': {
						...getPseudoElementStyles(
							theme,
							theme.spaces.s,
							LAST_RANGE_RIGHT_SPACE + theme.spaces.s
						),
					},
				}),
		}),

		...((isFirstDay || isFirstHoveredDay) &&
			completeRange &&
			!isSameDate && {
				'&:after': {
					...getPseudoElementStyles(
						theme,
						theme.spaces.s,
						FIRST_RANGE_RIGHT_SPACE
					),

					...(isLastDayOfWeek && {
						width: WIDTH_OF_RANGE_ELEMENT,
						right: -theme.spaces.s,
					}),
				},
			}),

		...((isLastDay || isLastHoveredDay) &&
			completeRange &&
			!isSameDate && {
				'&:before': {
					...getPseudoElementStyles(
						theme,
						theme.spaces.s,
						LAST_RANGE_RIGHT_SPACE
					),

					...(isFirstDayOfWeek && {
						width: WIDTH_OF_RANGE_ELEMENT,
					}),
				},
			}),

		...(isSameDate && {
			'&.Mui-disabled.MuiPickersDay-root.Mui-disabled:after, &.Mui-disabled.MuiPickersDay-root.Mui-disabled:before':
				{
					backgroundColor: 'transparent !important',
				},
		}),
	})
) as ComponentType<CustomPickersDayProps>;

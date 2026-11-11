import styled from '@emotion/styled';
import { CalendarUI } from '@mms/mms-ui-library/components/Calendar';
import { lightPalette } from '@mms/mms-ui-library/theme';

import { ThemeMode, WHITE } from '../../../../../../theme';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { DEFAULT_OPACITY, HALF_OPACITY } from '@/constants/opacities';
import { createGradient } from '@/helpers/createGradient';

import { DAY_CELL_SIZE } from './constants';

const BADGE_LIGHT_OPACITY = 0.6;
const BADGE_DARK_OPACITY = 0.2;
const RANGE_DATE_OPACITY = 0.7;
const BADGE_BOTTOM_SPACE = 1;
const BADGE_RIGHT_SPACE = '50%';
const CALENDAR_HEIGHT = 243;
const BADGE_DISABLE_OPACITY = 0.3;

export const CustomCalendarUI = styled(CalendarUI)(({ theme, disabled }) => ({
	height: CALENDAR_HEIGHT,
	flexShrink: 0,
	opacity: disabled ? HALF_OPACITY : DEFAULT_OPACITY,

	'#rangeDays': {
		background: `${createGradient(
			theme.palette['graphics-accent'],
			WHITE,
			theme.mode === ThemeMode.LIGHT ? RANGE_DATE_OPACITY : HALF_OPACITY
		)} !important`,
		borderRadius: BorderRadiuses.m,
		border: 'unset !important',
		color: `${lightPalette['text-black']} !important`,
	},

	'& .MuiDayCalendar-slideTransition': {
		boxSizing: 'content-box',
		overflow: 'visible',
		minHeight: 0,
		padding: 0,
	},

	'& .MuiDayCalendar-monthContainer': {
		overflow: 'visible',
	},

	'.MuiBadge-root': {
		boxSizing: 'unset',
		display: 'flex',
		justifyContent: 'center',
		width: `${DAY_CELL_SIZE}px`,
		height: `${DAY_CELL_SIZE}px`,
		lineHeight: 0,
		position: 'relative',
		zIndex: 1,

		'& .MuiPickersDay-root': {
			margin: 0,
			height: '100%',
			width: '100%',

			'&.Mui-disabled, &.Mui-disabled:not(.Mui-selected)': {
				opacity: DEFAULT_OPACITY,
			},

			'&.MuiPickersDay-dayOutsideMonth.Mui-disabled:not(.Mui-selected)': {
				opacity: BADGE_DISABLE_OPACITY,
			},
		},
	},

	'.MuiBadge-badge': {
		background: createGradient(
			theme.palette['graphics-accent'],
			WHITE,
			theme.mode === ThemeMode.DARK ? BADGE_DARK_OPACITY : BADGE_LIGHT_OPACITY
		),
		border: 'none',
		width: theme.spaces.xs,
		height: theme.spaces.xs,
		minWidth: theme.spaces.xs,
		top: 'auto',
		bottom: BADGE_BOTTOM_SPACE,
		right: BADGE_RIGHT_SPACE,
	},

	'.MuiBadge-root:has(.MuiPickersDay-today, .Mui-selected, [id="rangeDays"]) .MuiBadge-badge':
		{
			background: theme.palette['graphics-white'],
		},

	'.MuiDayCalendar-weekContainer': {
		width: '100%',
		display: 'flex',
		justifyContent: 'space-between',
		boxSizing: 'border-box',
	},
}));

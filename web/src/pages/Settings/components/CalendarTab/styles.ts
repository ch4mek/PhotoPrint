import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { SettingsPageGridWrapper } from '../../styles';

import { overrideGridStyles } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { cellBottomBorderStyles } from '@/components/Grid/styles';
import { BorderRadiuses } from '@/constants/borderRadiuses';
import { MONTH_IN_YEAR, PERIOD } from '@/constants/date';
import { getHoverBorderColor } from '@/helpers/getColor';
import { HOVER_ROW_CELL_CLASS_NAME } from '@/pages/TimeSheet/components/TimeSheeetManager/constants';
import { getColumnsHoverStyles } from '@/pages/TimeSheet/components/Timesheet/styles';
import { ThemeMode, TotalColumnColor } from '@/theme';

import {
	ADD_BUTTON_CELL_CLASS_NAME,
	CALENDAR_COUNTRY_COLUMN_CELL_CLASS,
	CALENDAR_COUNTRY_COLUMN_HEADER_CLASS,
	CALENDAR_TOTAL_COLUMN_CELL_CLASS,
	CALENDAR_TOTAL_COLUMN_HEADER_CLASS,
	desktopCalendarGridWidths,
} from './constants';
import { CalendarGridColumnWidths } from './types';

const FROZEN_TOTAL_COLUMN_INDEX = 15;
const COUNTRY_COLUMN_INDEX = 1;
const COUNTRY_COLUMN_INDEX_IF_TOTAL_IS_FROZEN = 2;
const LAST_MONTH_COLUMN_INDEX = 13;
const GRID_MIN_HEIGHT = 173;

const getCountryColumnIndex = (hasFrozenColumns: boolean) =>
	hasFrozenColumns
		? COUNTRY_COLUMN_INDEX_IF_TOTAL_IS_FROZEN
		: COUNTRY_COLUMN_INDEX;

const getHoveredColumnSelector = (hoverIndex: number) =>
	`div.rdg-cell:not([role='columnheader']):has(> [data-hovercolindex='${hoverIndex}'])`;

const getTotalColumnBackgroundColor = (theme: Theme) =>
	theme.mode === ThemeMode.LIGHT
		? theme.palette['background-primary-1']
		: TotalColumnColor.DARK;

const getCalendarHorizontalScrollbarStyles = (
	theme: Theme,
	columnsWidth: CalendarGridColumnWidths,
	isTotalFrozen: boolean
) => css`
	::-webkit-scrollbar {
		border-right: 1px solid ${getTotalColumnBackgroundColor(theme)};
	}

	::-webkit-scrollbar-track {
		border-left: ${columnsWidth.countryName}px solid
			${theme.palette['background-white']};
		border-right: ${isTotalFrozen ? columnsWidth.total : 0}px solid
			${getTotalColumnBackgroundColor(theme)};
	}

	::-webkit-scrollbar-track-piece:horizontal {
		margin-left: ${columnsWidth.countryName}px;
		background: ${theme.palette['background-white']};
	}

	::-webkit-scrollbar-track-piece:horizontal:end {
		margin-right: ${isTotalFrozen ? columnsWidth.total : 0}px;
	}

	::-webkit-scrollbar-corner {
		border-radius: 0;
		background-color: ${getTotalColumnBackgroundColor(theme)};
	}
`;

const getCalendarColumnGridTemplate = (
	columnsWidth: CalendarGridColumnWidths,
	hideFirstColumn: boolean
) => css`
	display: grid;
	grid-template-columns:
		${hideFirstColumn ? '0px' : ''} ${columnsWidth.countryName}px repeat(
			${PERIOD.length},
			minmax(${columnsWidth.month}px, 1fr)
		)
		${columnsWidth.total}px !important;
`;

const totalColumnFrozenStyles = css`
	right: -0.1px;
	position: sticky !important;
	box-shadow: none;
	grid-column: ${FROZEN_TOTAL_COLUMN_INDEX} !important;
`;

const countryColumnFrozenStyles = (hasFrozenColumns: boolean) => css`
	box-shadow: none;
	grid-column: ${getCountryColumnIndex(hasFrozenColumns)} !important;
`;

const lastMonthColumnBorderStyles = () => css`
	${getHoveredColumnSelector(LAST_MONTH_COLUMN_INDEX)} {
		border-right: none;
	}
`;

const getInputColorsStyles = (color: string) => css`
	.rdg-cell.${CALENDAR_COUNTRY_COLUMN_CELL_CLASS} {
		& > div:not(.${ADD_BUTTON_CELL_CLASS_NAME}) {
			label {
				background-color: ${color};
				border-radius: ${BorderRadiuses.xs2}px;
			}

			& > div > div {
				background-color: ${color};
			}

			& > div > div > div:has(span) {
				background-color: ${color} !important;
				border-radius: ${BorderRadiuses.xs2}px;
			}
		}
	}
`;

export const overrideCalendarGridStyles =
	(isCountryColumnFrozen: boolean, isTotalColumnFrozen: boolean) =>
	(theme: Theme) =>
		css`
			${overrideGridStyles(false, undefined, false)(theme).styles};
			${(isTotalColumnFrozen || isCountryColumnFrozen) &&
			getCalendarColumnGridTemplate(
				desktopCalendarGridWidths,
				isTotalColumnFrozen
			)};

			& .rdg-cell {
				justify-content: center;
				padding: 0;
				background-color: ${theme.palette['background-white']};
			}

			& .rdg-cell[role='columnheader'] {
				padding: 0;
				font-size: ${theme.typography.body['s-600'].fontSize};
				font-weight: ${theme.typography.body['s-600'].fontWeight};
			}

			&
				.rdg-cell.${CALENDAR_TOTAL_COLUMN_HEADER_CLASS},
				&
				.rdg-cell.${CALENDAR_TOTAL_COLUMN_CELL_CLASS} {
				padding: 0;
				border-right: none;
				border-left: 1px solid ${theme.palette['graphics-divider-1']};
				display: flex;
				justify-content: center;
				background-color: ${getTotalColumnBackgroundColor(theme)};
				${isTotalColumnFrozen && totalColumnFrozenStyles.styles}
			}

			&
				.rdg-cell.${CALENDAR_COUNTRY_COLUMN_HEADER_CLASS},
				&
				.rdg-cell.${CALENDAR_COUNTRY_COLUMN_CELL_CLASS} {
				${isCountryColumnFrozen &&
				countryColumnFrozenStyles(isTotalColumnFrozen).styles}
			}

			& .rdg-cell.${CALENDAR_TOTAL_COLUMN_HEADER_CLASS} {
				border-left: 1px solid ${theme.palette['graphics-input-stroke']};
				background-color: ${theme.mode === ThemeMode.LIGHT
					? theme.palette['background-primary-1']
					: theme.palette['background-base-2']};
			}

			& .rdg-cell[role='columnheader'] {
				&::after {
					${cellBottomBorderStyles(theme.palette['graphics-input-stroke'])
						.styles};
				}
			}

			& > div {
				${lastMonthColumnBorderStyles().styles}
			}

			${isCountryColumnFrozen &&
			getCalendarHorizontalScrollbarStyles(
				theme,
				desktopCalendarGridWidths,
				isTotalColumnFrozen
			).styles}

			${getInputColorsStyles(theme.palette['background-white'])};
		`;

export const CalendarContentWrapper = styled(SettingsPageGridWrapper)(
	({ theme }) => ({
		display: 'flex',
		gap: theme.spaces.m,
		flexDirection: 'column',
		height: '100%',
	})
);

export const GridWrapper = styled(DataGridHoverRowWrapper)<{
	hoveredColumn: number | null;
}>(
	({ theme }) =>
		css`
			display: contents;
			max-height: ${GRID_MIN_HEIGHT}px;

			.rdg-row .${HOVER_ROW_CELL_CLASS_NAME} {
				border-top: 1px solid transparent;
				border-bottom: 1px solid transparent;
			}

			.rdg-row:hover .${HOVER_ROW_CELL_CLASS_NAME} {
				border-top-color: ${getHoverBorderColor(theme)};
				border-bottom-color: ${getHoverBorderColor(theme)};
			}

			${getColumnsHoverStyles(MONTH_IN_YEAR, theme)}

			& > div {
				height: unset;
				min-height: ${GRID_MIN_HEIGHT}px;
			}
		`
);

export const CalendarTabControlsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
}));

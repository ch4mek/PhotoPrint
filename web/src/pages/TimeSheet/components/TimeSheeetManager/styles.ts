import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';

import {
	ADD_TASK_CELL_CLASS_NAME,
	TASK_INFO_CELL_CLASS_NAME,
	timesheetZIndexes,
	TOTAL_COLUMN_CELL_CLASS_NAME,
	TOTAL_COLUMN_HEADER_CLASS_NAME,
} from '../../constants';
import { isMonthPeriod } from '../../utils';
import { getTimesheetHorizontalScrollbarStyles } from '../styles';
import { TIMESHEET_EXPANDED_ROW_HEIGHT } from '../TimesheetEngineer/constants';

import { overrideGridStyles } from '@/components/Grid';
import { CROSS_LINK_CELL_CLASS } from '@/components/Grid/constants';
import { commonTimesheetGridStyles } from '@/components/Grid/utils/getGridColumnTemplateFromHeader';
import { createGradient } from '@/helpers/createGradient';
import {
	getOddRowBackgroundColor,
	getEvenRowBackgroundColor,
	createBorderColor,
	createTransparentBorderColor,
	getTotalColumnBackgroundColor,
	createActiveBorderColor,
	getHoverBorderColor,
} from '@/helpers/getColor';
import { WHITE } from '@/theme';

import {
	MANAGER_PERIOD_TOTAL_CELL,
	MANAGER_TIMESHEET_ROW_WRAPPER_CLASS_NAME,
	MANAGER_TOTAL_COLUMN_HEADER_CLASS_NAME,
	PRIMARY_ROW_TOTAL_CELL_CLASS_NAME,
	PRIMARY_ROW_USER_CELL_CLASS_NAME,
	HOVER_ROW_CELL_CLASS_NAME,
	USER_GRID_WRAPPER_CLASS_NAME,
	USER_NAME_COLUMN_HEADER_CLASS_NAME,
	USER_TOTAL_ROW_WRAPPER_CLASS_NAME,
} from './constants';

const MOBILE_LINE_HEIGHT_OFFSET = 2;
const TIMESHEET_GRADIENT_OPACITY = 40;

const freezedColumnsStyles = (
	theme: Theme,
	periodLength: number,
	isMobileView: boolean,
	rowHeight: number
) => css`
	& .${MANAGER_TIMESHEET_ROW_WRAPPER_CLASS_NAME} {
		display: contents;

		& > div {
			${commonTimesheetGridStyles(periodLength, isMobileView, true)(theme)
				.styles}
			display: grid;
			grid-template-rows: ${rowHeight}px 0px !important;
			border-bottom: 1px solid ${theme.palette['graphics-divider-2']};
		}

		.${USER_TOTAL_ROW_WRAPPER_CLASS_NAME} {
			display: contents;
		}

		& .${TOTAL_COLUMN_HEADER_CLASS_NAME} {
			grid-area: 1 / ${periodLength + 3} / 2 / ${periodLength + 4} !important;
		}

		& .${TOTAL_COLUMN_CELL_CLASS_NAME} {
			grid-column: ${periodLength + 3} / ${periodLength + 4} !important;
		}

		& .${TASK_INFO_CELL_CLASS_NAME}, .${ADD_TASK_CELL_CLASS_NAME} {
			position: sticky;
			inset-inline-start: 0px;
			z-index: ${timesheetZIndexes.taskCell};
		}

		.${PRIMARY_ROW_USER_CELL_CLASS_NAME} {
			grid-column: 2;
			grid-row: 1;
			position: sticky;
			inset-inline-start: 0px;
			background-color: inherit;
		}
	}

	& .${MANAGER_TOTAL_COLUMN_HEADER_CLASS_NAME} {
		right: -0.1px;
		position: sticky !important;
		grid-area: 1 / ${periodLength + 3} / 2 / ${periodLength + 4} !important;
	}

	& .${USER_NAME_COLUMN_HEADER_CLASS_NAME} {
		position: sticky;
		z-index: ${timesheetZIndexes.fixedCellWithPriority};
		inset-inline-start: 0;
	}

	& > .rdg-row > .rdg-cell[aria-colindex='1'] {
		display: none;
	}

	.${USER_GRID_WRAPPER_CLASS_NAME} {
		grid-column: 1 / 10;
		display: contents;

		& > div,
		.rdg {
			display: contents;
		}
	}
`;

const frozenRowsStyles = (headerHeight: number) => css`
	.${USER_TOTAL_ROW_WRAPPER_CLASS_NAME} > div {
		position: sticky;
		top: ${headerHeight}px;
		z-index: ${timesheetZIndexes.fixedCell};
	}

	.${USER_TOTAL_ROW_WRAPPER_CLASS_NAME}
		> .${PRIMARY_ROW_TOTAL_CELL_CLASS_NAME} {
		z-index: ${timesheetZIndexes.fixedCellWithPriority};
	}

	.${USER_TOTAL_ROW_WRAPPER_CLASS_NAME} {
		position: sticky;
		top: ${headerHeight}px;
		z-index: ${timesheetZIndexes.fixedCell} !important;
	}

	.${MANAGER_TIMESHEET_ROW_WRAPPER_CLASS_NAME} {
		overflow: clip;
	}

	.${USER_NAME_COLUMN_HEADER_CLASS_NAME},
		.${MANAGER_TOTAL_COLUMN_HEADER_CLASS_NAME} {
		z-index: ${timesheetZIndexes.fixedHeader} !important;
	}

	.${PRIMARY_ROW_USER_CELL_CLASS_NAME} {
		z-index: ${timesheetZIndexes.fixedCellWithPriority} !important;
	}
`;

export const overrideManagerTimesheetStyles =
	(
		periodLength: number,
		isMobileView: boolean,
		areColumnsFrozen: boolean,
		rowHeight: number,
		headerHeight: number
	) =>
	(theme: Theme) =>
		css`
			${overrideGridStyles(false, undefined, false)(theme)}
			${commonTimesheetGridStyles(
				periodLength,
				isMobileView,
				areColumnsFrozen
			)(theme)}
			${areColumnsFrozen && getTimesheetHorizontalScrollbarStyles(theme)}

			.rdg-cell[role='columnheader'] {
				z-index: ${timesheetZIndexes.header};
			}

			& > .rdg-header-row > .rdg-cell {
				padding: 0;
				overflow: hidden;

				border-right: 1px solid ${createBorderColor(theme)};

				& > div {
					padding: 0 ${theme.spaces.s}px;
				}
			}

			& > .rdg-row > .rdg-cell {
				align-items: start;
				padding: 0px;
				border-right: none;
				line-height: ${rowHeight -
				(isMobileView ? MOBILE_LINE_HEIGHT_OFFSET : 0)}px;
			}

			.${TOTAL_COLUMN_CELL_CLASS_NAME} > div {
				background: ${createGradient(
					WHITE,
					getTotalColumnBackgroundColor(theme),
					TIMESHEET_GRADIENT_OPACITY
				)};
			}

			.${MANAGER_PERIOD_TOTAL_CELL}, .${PRIMARY_ROW_TOTAL_CELL_CLASS_NAME} {
				display: flex;
				justify-content: center;
			}

			.${USER_TOTAL_ROW_WRAPPER_CLASS_NAME} {
				${commonTimesheetGridStyles(periodLength, isMobileView, true)(theme)
					.styles};
				width: 100%;

				& > div {
					border-top: 1px solid ${createTransparentBorderColor()};
					border-bottom: 1px solid ${theme.palette['graphics-divider-2']};
					border-right: 1px solid ${theme.palette['graphics-divider-1']};
				}
			}

			.${USER_GRID_WRAPPER_CLASS_NAME} {
				.${TOTAL_COLUMN_CELL_CLASS_NAME} {
					border-right: 1px solid
						${isMonthPeriod(periodLength)
							? createActiveBorderColor(theme)
							: createTransparentBorderColor()} !important;
					border-left: 1px solid ${createActiveBorderColor(theme)} !important;
					z-index: ${timesheetZIndexes.fixedCell};
				}

				div:not(.${TOTAL_COLUMN_CELL_CLASS_NAME}):has(div[data-hovercolindex]) {
					border-right: 1px solid ${createActiveBorderColor(theme)};
				}

				.rdg-row:hover .${HOVER_ROW_CELL_CLASS_NAME} {
					border-top-color: ${getHoverBorderColor(theme)};
					border-bottom-color: ${getHoverBorderColor(theme)};
				}

				div[role='grid'] {
					overflow: hidden;
				}

				${isMobileView &&
				`.rdg-row:not(:has(+ :not(.rdg-row))) div[role='gridcell'] {
					height: ${TIMESHEET_EXPANDED_ROW_HEIGHT}px;
				}`}
			}

			.rdg-row {
				&-odd {
					.${USER_TOTAL_ROW_WRAPPER_CLASS_NAME} > div,
					.${USER_GRID_WRAPPER_CLASS_NAME} .rdg-cell > div {
						background-color: ${getOddRowBackgroundColor(theme)};
					}

					.${USER_GRID_WRAPPER_CLASS_NAME} .${HOVER_ROW_CELL_CLASS_NAME} {
						border-top-color: ${getOddRowBackgroundColor(theme)};
						border-bottom-color: ${getOddRowBackgroundColor(theme)};
					}
				}

				&-even {
					.rdg-cell > div,
					.${USER_TOTAL_ROW_WRAPPER_CLASS_NAME} > div,
					.${USER_GRID_WRAPPER_CLASS_NAME} .rdg-cell > div {
						background-color: ${getEvenRowBackgroundColor(theme)};
					}

					.${USER_GRID_WRAPPER_CLASS_NAME} .${HOVER_ROW_CELL_CLASS_NAME} {
						border-top-color: ${getEvenRowBackgroundColor(theme)};
						border-bottom-color: ${getEvenRowBackgroundColor(theme)};
					}

					.${PRIMARY_ROW_TOTAL_CELL_CLASS_NAME} {
						background: ${createGradient(
							WHITE,
							getTotalColumnBackgroundColor(theme),
							TIMESHEET_GRADIENT_OPACITY
						)};
					}
				}

				&:hover .${CROSS_LINK_CELL_CLASS} {
					text-decoration: underline;
					cursor: pointer;
				}
			}

			height: 100%;

			${areColumnsFrozen &&
			freezedColumnsStyles(theme, periodLength, isMobileView, rowHeight)}
			${frozenRowsStyles(headerHeight - 1).styles};
		`;

export const NameHeaderWrapper = styled('div')(() => ({
	display: 'flex',
	justifyContent: 'left',
	alignItems: 'center',
	width: '100%',
	height: '100%',
}));

export const DayHeaderWrapper = styled('div')(() => ({
	display: 'flex',
	justifyContent: 'center',
	alignItems: 'center',
	width: '100%',
	height: '100%',
}));

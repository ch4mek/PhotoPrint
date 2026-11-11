import { css, Theme } from '@emotion/react';

import {
	INTERVIEW_ROW_CLASSES,
	INTERVIEW_STATUS_COLORS,
	INTERVIEW_INACTIVE_STATUS_COLORS,
	interviewGrayStatusColor,
	interviewInactiveGrayStatusColor,
	STAGE_INTERVIEW_FONT_SIZE,
	TODAY_CELL_COLOR,
} from '../constants';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { WIDGET_GRID_HEIGHT } from '@/components/Grid/constants';
import { DEFAULT_DISABLED_GRID_CELL_OPACITY } from '@/constants/opacities';
import { getHoverBorderColor } from '@/helpers/getColor';
import { ThemeMode } from '@/theme';
import { GridColumnWidthsConfiguration } from '@/types/grid';

export const getOverrideInterviewGridStyles =
	(
		columnsWidths: GridColumnWidthsConfiguration,
		columnsLength: number,
		rowsLength: number,
		isWidget = false
	) =>
	(gridTheme: Theme) => {
		const getGridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			columnsWidths,
			!isWidget
		);

		const clearFiltersColumnIndex = columnsLength + 1;

		const extendedStyles = overrideGridStylesWithEditColumn(
			!!rowsLength,
			clearFiltersColumnIndex,
			getGridColumnTemplate
		);

		return css`
			${extendedStyles(gridTheme)}

			.rdg-row.${INTERVIEW_ROW_CLASSES.statusWaitingForInterview} {
				background-color: ${INTERVIEW_STATUS_COLORS.waitingForInterview[
					gridTheme.mode
				]};
			}

			.rdg-row.${INTERVIEW_ROW_CLASSES.statusWaitingForResult} {
				background-color: ${INTERVIEW_STATUS_COLORS.waitingForResult[
					gridTheme.mode
				]};
			}

			.rdg-row.${INTERVIEW_ROW_CLASSES.statusFailed} {
				background-color: ${INTERVIEW_STATUS_COLORS.failed[gridTheme.mode]};
			}

			.rdg-row.${INTERVIEW_ROW_CLASSES.statusSuccess} {
				background-color: ${INTERVIEW_STATUS_COLORS.success[gridTheme.mode]};
			}

			.rdg-row.${INTERVIEW_ROW_CLASSES.statusStarted} {
				background-color: ${INTERVIEW_STATUS_COLORS.started[gridTheme.mode]};
			}

			.rdg-row.${INTERVIEW_ROW_CLASSES.statusNotStarted} {
				background-color: ${INTERVIEW_STATUS_COLORS.notStarted[gridTheme.mode]};
			}
			.rdg-row.${INTERVIEW_ROW_CLASSES.statusCanceled},
				.rdg-row.${INTERVIEW_ROW_CLASSES.statusNoFeedback},
				.rdg-row.${INTERVIEW_ROW_CLASSES.statusNoNextStepScheduled} {
				background-color: ${interviewGrayStatusColor[gridTheme.mode]};
			}

			.${INTERVIEW_ROW_CLASSES.inActiveUser} {
				&.${INTERVIEW_ROW_CLASSES.statusWaitingForInterview} {
					background-color: ${INTERVIEW_INACTIVE_STATUS_COLORS
						.WaitingForInterview[gridTheme.mode]};
				}

				&.${INTERVIEW_ROW_CLASSES.statusWaitingForResult} {
					background-color: ${INTERVIEW_INACTIVE_STATUS_COLORS.WaitingForResult[
						gridTheme.mode
					]};
				}

				&.${INTERVIEW_ROW_CLASSES.statusFailed} {
					background-color: ${INTERVIEW_INACTIVE_STATUS_COLORS.Failed[
						gridTheme.mode
					]};
				}

				&.${INTERVIEW_ROW_CLASSES.statusSuccess} {
					background-color: ${INTERVIEW_INACTIVE_STATUS_COLORS.Success[
						gridTheme.mode
					]};
				}

				&.${INTERVIEW_ROW_CLASSES.statusStarted} {
					background-color: ${INTERVIEW_INACTIVE_STATUS_COLORS.Started[
						gridTheme.mode
					]};
				}

				&.${INTERVIEW_ROW_CLASSES.statusNotStarted} {
					background-color: ${INTERVIEW_INACTIVE_STATUS_COLORS.NotStarted[
						gridTheme.mode
					]};
				}

				&.${INTERVIEW_ROW_CLASSES.statusCanceled},
					&.${INTERVIEW_ROW_CLASSES.statusNoFeedback},
					&.${INTERVIEW_ROW_CLASSES.statusNoNextStepScheduled} {
					background-color: ${interviewInactiveGrayStatusColor[gridTheme.mode]};
				}

				& div div {
					opacity: ${DEFAULT_DISABLED_GRID_CELL_OPACITY};
				}
			}

			.rdg-row.${INTERVIEW_ROW_CLASSES.dateToday} .interview-date-cell {
				background-color: ${TODAY_CELL_COLOR[gridTheme.mode]};
			}

			.rdg-row.${INTERVIEW_ROW_CLASSES.stageRow} > .rdg-cell {
				font-size: ${STAGE_INTERVIEW_FONT_SIZE};
			}

			.rdg-row:hover > .rdg-cell {
				background-color: inherit !important;
			}

			.rdg-row.${INTERVIEW_ROW_CLASSES.dateToday}:hover .interview-date-cell {
				background-color: ${TODAY_CELL_COLOR[gridTheme.mode]} !important;
			}

			.rdg-row:hover .rdg-cell[role='gridcell'] {
				&::before {
					content: '';
					position: absolute;
					inset: 0;
					border-block: 1px solid ${getHoverBorderColor(gridTheme)};
					pointer-events: none;
				}

				&::after {
					height: 0px;
				}
			}

			.rdg-row:has(+ .rdg-row:hover) .rdg-cell[role='gridcell']::after {
				height: 0px;
			}

			${gridTheme.mode === ThemeMode.DARK &&
			css`
				.rdg-row.${INTERVIEW_ROW_CLASSES.statusWaitingForResult} *,
				.rdg-row.${INTERVIEW_ROW_CLASSES.statusFailed} *,
				.rdg-row.${INTERVIEW_ROW_CLASSES.statusSuccess} *,
				.rdg-row.${INTERVIEW_ROW_CLASSES.statusStarted} *,
				.rdg-row.${INTERVIEW_ROW_CLASSES.dateToday} .interview-date-cell {
					color: ${gridTheme.palette['text-white']} !important;
				}

				.rdg-row.${INTERVIEW_ROW_CLASSES.inActiveUser}.${INTERVIEW_ROW_CLASSES.statusWaitingForResult}
					*,
				.rdg-row.${INTERVIEW_ROW_CLASSES.inActiveUser}.${INTERVIEW_ROW_CLASSES.statusFailed}
					*,
				.rdg-row.${INTERVIEW_ROW_CLASSES.inActiveUser}.${INTERVIEW_ROW_CLASSES.statusSuccess}
					*,
				.rdg-row.${INTERVIEW_ROW_CLASSES.inActiveUser}.${INTERVIEW_ROW_CLASSES.statusStarted}
					* {
					color: ${gridTheme.palette['text-black']} !important;
				}

				.rdg-row.${INTERVIEW_ROW_CLASSES.dateToday}.${INTERVIEW_ROW_CLASSES.inActiveUser}
					.interview-date-cell
					* {
					color: ${gridTheme.palette['text-white']} !important;
				}
			`}

			${isWidget &&
			css`
				height: ${`${WIDGET_GRID_HEIGHT}px`};
			`}
		`;
	};

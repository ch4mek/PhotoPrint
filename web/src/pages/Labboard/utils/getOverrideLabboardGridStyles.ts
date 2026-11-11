import { css, Theme } from '@emotion/react';

import {
	LABBOARD_COLOR_MIX_WHITE_PERCENTAGE,
	LABBOARD_PLANNED_END_CELL_CLASS,
	LABBOARD_ROW_CLASSES,
} from '../constants';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DEFAULT_DISABLED_GRID_CELL_OPACITY } from '@/constants/opacities';
import { getHoverBorderColor } from '@/helpers/getColor';
import { GRID_FROZEN_EDIT_COLUMN_DIFF } from '@/pages/constants';
import { ThemeMode } from '@/theme';
import { GridColumnWidthsConfiguration } from '@/types/grid';

const getLabboardColorMix = (theme: Theme, color: string) =>
	`color-mix(in srgb, ${
		theme.mode === ThemeMode.LIGHT
			? theme.palette['graphics-white']
			: theme.palette['even-stroke-base-1']
	} ${LABBOARD_COLOR_MIX_WHITE_PERCENTAGE}%, ${color})`;

export const getOverrideLabboardGridStyles =
	(
		columnsWidths: GridColumnWidthsConfiguration,
		columnsAmount: number,
		rowsLength: number
	) =>
	(gridTheme: Theme) => {
		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			columnsWidths,
			true
		);

		const extendedStyles = overrideGridStylesWithEditColumn(
			!!rowsLength,
			columnsAmount + GRID_FROZEN_EDIT_COLUMN_DIFF,
			gridColumnTemplate
		);

		const successColor = getLabboardColorMix(
			gridTheme,
			gridTheme.palette['feedback-success']
		);
		const firedColor = getLabboardColorMix(
			gridTheme,
			gridTheme.palette['graphics-error']
		);
		const overdueCellColor = getLabboardColorMix(
			gridTheme,
			gridTheme.palette['graphics-yellow']
		);

		return css`
			${extendedStyles(gridTheme)}

			.rdg-row.${LABBOARD_ROW_CLASSES.finalResultSuccess} {
				background-color: ${successColor};
			}

			.rdg-row.${LABBOARD_ROW_CLASSES.finalResultFired} {
				background-color: ${firedColor};
			}

			.${LABBOARD_ROW_CLASSES.inactiveUser} {
				& div div {
					opacity: ${DEFAULT_DISABLED_GRID_CELL_OPACITY};
				}
			}

			.rdg-row.${LABBOARD_ROW_CLASSES.plannedEndOverdue}
				.${LABBOARD_PLANNED_END_CELL_CLASS} {
				background-color: ${overdueCellColor};
			}

			.rdg-row:hover > .rdg-cell {
				background-color: inherit !important;
			}

			.rdg-row.${LABBOARD_ROW_CLASSES.plannedEndOverdue}:hover
				.${LABBOARD_PLANNED_END_CELL_CLASS} {
				background-color: ${overdueCellColor} !important;
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
		`;
	};

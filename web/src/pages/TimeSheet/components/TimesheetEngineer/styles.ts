import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { breakpoints } from '@mms/mms-ui-library/theme';

import {
	ADD_TASK_CELL_CLASS_NAME,
	TASK_INFO_CELL_CLASS_NAME,
	TASK_INFO_HEADER_CLASS_NAME,
	timesheetZIndexes,
	TOTAL_COLUMN_CELL_CLASS_NAME,
	TOTAL_COLUMN_HEADER_CLASS_NAME,
	TOTAL_ROW_CELL_CLASS_NAME,
} from '../../constants';
import { getTimesheetHorizontalScrollbarStyles } from '../styles';
import { HOVER_ROW_CELL_CLASS_NAME } from '../TimeSheeetManager/constants';

import { overrideGridStyles } from '@/components/Grid';
import {
	GRID_ROW_WITH_ADD_BUTTON_HEIGHT,
	GRID_TIME_CELL_ROW_HEIGHT,
} from '@/components/Grid/constants';
import { commonTimesheetGridStyles } from '@/components/Grid/utils/getGridColumnTemplateFromHeader';
import { checkIfForwardProps } from '@/helpers/checkIfForwardProps';
import {
	createTransparentBorderColor,
	getHoverBorderColor,
	getTotalColumnBackgroundColor,
} from '@/helpers/getColor';

import { TIMESHEET_EXPANDED_ROW_HEIGHT } from './constants';

const { tablet } = breakpoints;
const TASK_INFO_CONTAINER_PROPS_TO_CHECK = ['isMobileView'];

const frozenColumnsStyles = (periodLenght: number) => css`
	& .${TOTAL_COLUMN_HEADER_CLASS_NAME}, .${TOTAL_COLUMN_CELL_CLASS_NAME} {
		right: -1px;
		box-shadow: none;
	}

	& .${TOTAL_COLUMN_HEADER_CLASS_NAME} {
		grid-area: 1 / ${periodLenght + 3} / 2 / ${periodLenght + 4} !important;
		z-index: ${timesheetZIndexes.fixedHeader};
	}

	& .${TOTAL_COLUMN_CELL_CLASS_NAME} {
		grid-column: ${periodLenght + 3} / ${periodLenght + 4} !important;
		z-index: ${timesheetZIndexes.fixedCell};
	}

	&
		.${TASK_INFO_CELL_CLASS_NAME},
		.${ADD_TASK_CELL_CLASS_NAME},
		.${TASK_INFO_HEADER_CLASS_NAME} {
		inset-inline-start: 0px;
		grid-column: 2 !important;
		box-shadow: none;
	}
`;

const frozenRowsStyles = (headerHeight: number) => (theme: Theme) =>
	css`
		.${TOTAL_ROW_CELL_CLASS_NAME} {
			position: sticky;
			top: ${headerHeight}px;
			z-index: ${timesheetZIndexes.header};
			border-bottom: 1px solid ${theme?.palette['graphics-divider-2']};

			&.rdg-cell-frozen {
				z-index: ${timesheetZIndexes.fixedHeader};
			}
		}
	`;

export const timeSheetEnginerGridStyles =
	(
		periodLength: number,
		isMobileView: boolean,
		areColumnsFrozen: boolean,
		headerHeight?: number
	) =>
	(theme: Theme) =>
		css`
			${overrideGridStyles(true, undefined, false)(theme)}
			${commonTimesheetGridStyles(
				periodLength,
				isMobileView,
				areColumnsFrozen
			)(theme)}
      ${areColumnsFrozen && getTimesheetHorizontalScrollbarStyles(theme)}
      ${areColumnsFrozen && frozenColumnsStyles(periodLength)}
      
			overflow: auto;
			block-size: auto;
			height: 100%;

			.rdg-row {
				margin: ${theme.spaces.m}px 0;

				& > .${HOVER_ROW_CELL_CLASS_NAME} {
					border-top: 1px solid transparent;
					border-bottom: 1px solid transparent;
				}

				&:hover .${HOVER_ROW_CELL_CLASS_NAME} {
					border-top-color: ${getHoverBorderColor(theme)} !important;
					border-bottom-color: ${getHoverBorderColor(theme)} !important;
				}
			}

			${`.${TASK_INFO_CELL_CLASS_NAME}:not(.${TOTAL_ROW_CELL_CLASS_NAME})`} {
				padding: 0;
				${areColumnsFrozen && `height: ${GRID_TIME_CELL_ROW_HEIGHT}px;`}

				${areColumnsFrozen && theme.breakpoints.down(tablet)} {
					height: ${TIMESHEET_EXPANDED_ROW_HEIGHT}px;
				}

				&.rdg-cell > div {
					${theme.breakpoints.down(tablet)} {
						justify-content: flex-start;
					}
				}
			}

			.${ADD_TASK_CELL_CLASS_NAME} {
				${areColumnsFrozen &&
				`min-height: ${GRID_ROW_WITH_ADD_BUTTON_HEIGHT}px;`}
			}

			.rdg-cell-frozen-last {
				border-right: 1px solid ${createTransparentBorderColor()};
			}

			& .${TASK_INFO_CELL_CLASS_NAME}, .${ADD_TASK_CELL_CLASS_NAME} {
				z-index: ${timesheetZIndexes.cell};
			}

			& .${TASK_INFO_HEADER_CLASS_NAME} {
				z-index: ${timesheetZIndexes.fixedHeader};
			}

			.rdg-cell {
				padding: 0 !important;
				border-bottom: 0;

				&,
				& > div {
					display: flex;
					justify-content: center;
					align-items: center;
				}

				& span {
					text-overflow: ellipsis;
					width: auto;
				}

				&[role='columnheader']:not(
						.${TOTAL_COLUMN_HEADER_CLASS_NAME}, .${TASK_INFO_HEADER_CLASS_NAME}
					) {
					z-index: ${timesheetZIndexes.header};
				}
			}

			.rdg-row-odd {
				.rdg-cell {
					background-color: ${theme.palette['background-white']};
				}
				.${TOTAL_COLUMN_CELL_CLASS_NAME} {
					background-color: ${getTotalColumnBackgroundColor(theme)};
				}
			}

			height: 100%;
			${headerHeight && frozenRowsStyles(headerHeight)(theme)}
		`;

const FullWidthWrapper = styled('div')(
	() => css`
		display: flex;
		width: 100%;
	`
);

export const AddButtonWrapper = styled(FullWidthWrapper)(
	({ theme }) => css`
		padding-left: ${theme.spaces.s}px;

		& > div {
			display: flex;
		}
	`
);

export const TaskInfoContainer = styled(FullWidthWrapper, {
	shouldForwardProp: checkIfForwardProps(TASK_INFO_CONTAINER_PROPS_TO_CHECK),
})<{
	isMobileView: boolean;
}>(
	({ theme, isMobileView }) => css`
		padding: 0 ${theme.spaces.s}px;
		${isMobileView
			? css`
					justify-self: flex-start;
					width: min-content;
					flex-direction: column;
					justify-content: center;
					align-items: flex-end;
					gap: ${theme.spaces.xl}px;

					& > div {
						width: fit-content;
					}

					& > div:nth-of-type(2) {
						width: 171px;
					}
			  `
			: css`
					gap: ${theme.spaces.s}px;
			  `}
	`
);

export const FullWidthAndHeightWrapper = styled('div')`
	width: 100%;
	height: 100%;
`;

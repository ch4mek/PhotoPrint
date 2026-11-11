import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

import {
	DAYS_COLUMNS_START_INDEX,
	TOTAL_COLUMN_CELL_CLASS_NAME,
} from '../../constants';
import { getHoverColumnIndexByOrder } from '../../utils/getHoverColumnIndexByOrder';
import { getTimesheetColumnClassname } from '../../utils/getTimesheetColumnClassname';
import {
	HOVER_NAME_CELL_CLASS_NAME,
	HOVER_TOTAL_CELL_CLASS_NAME,
	USER_GRID_WRAPPER_CLASS_NAME,
} from '../TimeSheeetManager/constants';

import {
	getHoverBorderColor,
	getTotalColumnHoverBackgroundColor,
} from '@/helpers/getColor';

export const getColumnsHoverStyles = (periodLength: number, theme: Theme) =>
	new Array(getHoverColumnIndexByOrder(periodLength)).fill(0).reduce(
		(hovers, _, columnIndex) => `
	${hovers}
	&:has(.${getTimesheetColumnClassname(columnIndex, true)}:hover) {
		.${getTimesheetColumnClassname(columnIndex, true)} {
			border-right-color: ${getHoverBorderColor(theme)} !important;
		}

		.${
			columnIndex > DAYS_COLUMNS_START_INDEX
				? getTimesheetColumnClassname(columnIndex - 1, true)
				: `${HOVER_NAME_CELL_CLASS_NAME}`
		}  {
			border-right-color: ${getHoverBorderColor(theme)} !important;
		}

		${
			columnIndex === getHoverColumnIndexByOrder(periodLength) - 1 &&
			`
			.${getTimesheetColumnClassname(columnIndex - 1, true)} {
				border-right-color: ${getHoverBorderColor(theme)} !important;
			}
		`
		}
	}
`,
		`
		.${HOVER_TOTAL_CELL_CLASS_NAME} {
			border-right: none !important;
		}
		`
	);

export const ManagerGridWrapper = styled('div')<{ periodLength: number }>(
	({ theme, periodLength }) => `
  	display: contents;
	
	.${USER_GRID_WRAPPER_CLASS_NAME}:hover {
		.rdg-row:hover .${TOTAL_COLUMN_CELL_CLASS_NAME} > div:not(:empty) {
			background: ${getTotalColumnHoverBackgroundColor(theme)};
		}
	}

	${getColumnsHoverStyles(periodLength, theme)}
`
);

export const EmployeeGridWrapper = styled('div')<{ periodLength: number }>(
	({ theme, periodLength }) => `
  	display: contents;

	&:hover {
		.rdg-row:hover .${TOTAL_COLUMN_CELL_CLASS_NAME} > div:not(:empty) {
			background: ${getTotalColumnHoverBackgroundColor(theme)};
		}
	}

	${getColumnsHoverStyles(periodLength, theme)}
`
);

import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { timeSheetEnginerGridStyles } from '../../../TimesheetEngineer/styles';
import { HOVER_ROW_CELL_CLASS_NAME } from '../../constants';

import {
	getHoverBorderColor,
	getTaskInfoColumnBackgroundColor,
} from '@/helpers/getColor';
import { getInputColorsStyles } from '@/pages/TimeSheet/utils/getInputLabelsColorStyles';

export const overrideEngineerTimesheetStyle =
	(periodLength: number, isMobileView: boolean, areColumnsFrozen: boolean) =>
	(theme: Theme) =>
		css`
			${timeSheetEnginerGridStyles(
				periodLength,
				isMobileView,
				areColumnsFrozen
			)(theme)};
			${getInputColorsStyles(getTaskInfoColumnBackgroundColor(theme))}

			border: none;
			height: 100%;

			&::before {
				content: none;
			}
		`;

export const UserTimesheetWrapper = styled('div')(`
	box-sizing: border-box;
    overflow: hidden;
    width: 100%;
  `);

export const UserTimesheetGrid = styled('div')`
	width: 100%;
`;

export const TimeSheetEngineerGridWrapper = styled('div')`
	height: fit-content;

	& > div {
		border: none;
		border-radius: 0;

		.rdg-header-row {
			display: none;
		}
	}
`;

export const RowWrapper = styled('div')`
	${({ theme }) => css`
		overflow: hidden;
		cursor: pointer;

		&:hover .${HOVER_ROW_CELL_CLASS_NAME} {
			border-top-color: ${getHoverBorderColor(theme)};
			border-bottom-color: ${getHoverBorderColor(theme)};
		}
	`}
`;

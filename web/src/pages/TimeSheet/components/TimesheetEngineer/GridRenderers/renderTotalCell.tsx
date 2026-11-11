import { css, useTheme } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { breakpoints } from '@mms/mms-ui-library/theme';

import { HOVER_TOTAL_CELL_CLASS_NAME } from '../../TimeSheeetManager/constants';
import { ADD_ROW_ID, DEFAULT_TOTAL_TIME, TOTAL_ROW_ID } from '../constants';
import { TimeSheetRow } from '../types';

import { minutesToHours } from '@/helpers/minutesToHours';
import { getTotalPeriodTrackedTime } from '@/pages/TimeSheet/utils/getTotalPeriodTrackedTime';
import { RenderCellPropsWithHoverIndex } from '@/types/columnHoverTypes';

const getTotalCellGap = (isTotalRow: boolean) => (isTotalRow ? '4px' : '5px');
const getTabletTotalCellGap = (isTotalRow: boolean) =>
	isTotalRow ? `gap: 0px;` : '';

const getTotalCellPadding = (isTotalRow: boolean, isDoubleCell?: boolean) =>
	!isTotalRow ? `padding-bottom: ${isDoubleCell ? 40 : 65}px;` : '';

const TotalCellWrapper = styled('div')<{
	isTotalRow: boolean;
	isDoubleCell?: boolean;
}>(
	({ theme, isTotalRow, isDoubleCell }) => css`
		width: 100%;
		height: 100%;

		& > div {
			width: 100%;
			height: 100%;
			display: flex;
			flex-direction: column;
			align-items: center;
			justify-content: center;
			gap: ${getTotalCellGap(isTotalRow)};
		}

		${theme.breakpoints.down(breakpoints.tablet)} {
			font-size: ${theme.typography.pxToRem(14)};
			${getTotalCellPadding(isTotalRow, isDoubleCell)}

			& > div {
				${getTabletTotalCellGap(isTotalRow)}
			}
		}
	`
);

export const renderTotalCell = (
	props: RenderCellPropsWithHoverIndex<TimeSheetRow>
) => {
	const {
		row: { timeMap, rowId, displayTotalRow, isDoubleTimeCell },
		column: { hoverIndex },
	} = props;

	const theme = useTheme();

	if (rowId === TOTAL_ROW_ID && !displayTotalRow) {
		return null;
	}

	const isTotalRow = rowId === TOTAL_ROW_ID;

	if (rowId === ADD_ROW_ID) {
		return (
			<TotalCellWrapper
				isTotalRow={isTotalRow}
				data-hovercolindex={hoverIndex}
				className={HOVER_TOTAL_CELL_CLASS_NAME}
			/>
		);
	}

	const [totalPeriodFactTime, totalPeriodBillableTime] =
		getTotalPeriodTrackedTime(timeMap);

	if (isDoubleTimeCell) {
		return (
			<TotalCellWrapper
				isDoubleCell={isDoubleTimeCell}
				isTotalRow={isTotalRow}
				data-hovercolindex={hoverIndex}
			>
				<div>
					<Typography
						align="center"
						variant="s-400"
						color={theme.palette['base-6']}
					>
						{minutesToHours(totalPeriodFactTime) ||
							(rowId !== ADD_ROW_ID && DEFAULT_TOTAL_TIME)}
					</Typography>
					<Typography
						align="center"
						variant="s-400"
						color={theme.palette['base-6']}
					>
						{minutesToHours(totalPeriodBillableTime) ||
							(rowId !== ADD_ROW_ID && DEFAULT_TOTAL_TIME)}
					</Typography>
				</div>
			</TotalCellWrapper>
		);
	}

	return (
		<TotalCellWrapper
			isDoubleCell={isDoubleTimeCell}
			isTotalRow={isTotalRow}
			data-hovercolindex={hoverIndex}
		>
			<div>
				<Typography
					align="center"
					variant="s-400"
					color={theme.palette['base-6']}
				>
					{minutesToHours(totalPeriodFactTime) ||
						(rowId !== ADD_ROW_ID && DEFAULT_TOTAL_TIME)}
				</Typography>
			</div>
		</TotalCellWrapper>
	);
};

import { useMemo } from 'react';

import { DoubleTimeInputCell } from '../components/DoubleTimeInputCell';
import { EmptyCell } from '../components/EmptyCell';
import { TimeInputCell } from '../components/TimeInputCell';
import { TotalDayTimeCell } from '../components/TotalDayTimeCell.tsx';
import { ADD_ROW_ID, TOTAL_ROW_ID } from '../constants';
import { FullWidthAndHeightWrapper } from '../styles';
import { TimeSheetRow } from '../types';

import { getTimesheetColumnClassname } from '@/pages/TimeSheet/utils/getTimesheetColumnClassname';
import { RenderCellPropsWithHoverIndex } from '@/types/columnHoverTypes';

export function renderTrackCell(
	props: RenderCellPropsWithHoverIndex<TimeSheetRow, unknown>
) {
	const {
		column: { hoverIndex },
		row,
	} = props;
	const { rowId, displayTotalRow, isDoubleTimeCell } = row;

	if (displayTotalRow === false) {
		return null;
	}

	const cellContent = useMemo(() => {
		if (rowId === ADD_ROW_ID) {
			return <EmptyCell />;
		}

		if (rowId === TOTAL_ROW_ID) {
			return <TotalDayTimeCell {...props} />;
		}

		if (isDoubleTimeCell) {
			return <DoubleTimeInputCell {...props} />;
		}

		return <TimeInputCell {...props} />;
	}, [rowId, props, isDoubleTimeCell]);

	return (
		<FullWidthAndHeightWrapper
			data-hovercolindex={hoverIndex}
			{...(rowId !== ADD_ROW_ID && {
				className: getTimesheetColumnClassname(hoverIndex + 1, true),
			})}
		>
			{cellContent}
		</FullWidthAndHeightWrapper>
	);
}

import { useMemo } from 'react';

import { AddTaskCell } from '../components/AddTaskCell';
import { EmptyCell } from '../components/EmptyCell';
import { TaskInfoCell } from '../components/TaskInfoCell';
import { ADD_ROW_ID, TOTAL_ROW_ID } from '../constants';
import { FullWidthAndHeightWrapper } from '../styles';
import { TimeSheetRow } from '../types';

import { RenderCellPropsWithHoverIndex } from '@/types/columnHoverTypes';

export function renderTaskInfoCell(
	props: RenderCellPropsWithHoverIndex<TimeSheetRow, unknown>
) {
	const {
		column: { hoverIndex },
		row: { rowId },
	} = props;

	const cellContent = useMemo(() => {
		if (rowId === TOTAL_ROW_ID) {
			return <EmptyCell />;
		}

		if (rowId === ADD_ROW_ID) {
			return <AddTaskCell {...props} />;
		}

		return <TaskInfoCell {...props} />;
	}, [rowId, props]);

	return (
		<FullWidthAndHeightWrapper data-hovercolindex={hoverIndex}>
			{cellContent}
		</FullWidthAndHeightWrapper>
	);
}

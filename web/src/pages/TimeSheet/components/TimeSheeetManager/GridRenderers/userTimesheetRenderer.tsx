import { useCallback, useContext, useState, useEffect } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { TimeTrackItem } from '../../TimesheetEngineer/types';
import { UserTimesheet } from '../components/UserTimesheet';
import { TimeSheetManagerRow } from '../types';

import { TimesheetParametersContext } from '@/pages/TimeSheet/context/timesheetParametersContext';
import { TimesheetStatusType } from '@/pages/TimeSheet/types';

export const userTimesheetRenderer = (
	props: RenderCellProps<TimeSheetManagerRow, unknown>
) => {
	const { row, onRowChange } = props;
	const { period } = useContext(TimesheetParametersContext);
	const [updatedRow, setUpdatedRow] = useState(row);

	const updateRow = useCallback(
		(rowProperties: Partial<TimeSheetManagerRow>) => {
			setUpdatedRow((prevRow) => ({ ...prevRow, ...rowProperties }));
		},
		[]
	);

	useEffect(() => {
		if (updatedRow !== row) {
			onRowChange(updatedRow);
		}
	}, [updatedRow, row]);

	const setHeight = useCallback(
		(height: number) => {
			updateRow({ userTimesheetRowHeight: height });
		},
		[updateRow]
	);

	const setIsExpanded = (isExpanded: boolean) => {
		updateRow({ isExpanded });
	};

	const setTotalTimeMapChange = (
		newTimeMap: Map<string, undefined | TimeTrackItem>
	) => {
		updateRow({ timeMap: newTimeMap });
	};

	const onTimesheetStatusChange = (status: TimesheetStatusType) => {
		updateRow({ status, shouldDisplaySkeleton: true });
	};

	return (
		<UserTimesheet
			onRowHeightChange={setHeight}
			onExpandedChange={setIsExpanded}
			onTotalTimeMapChange={setTotalTimeMapChange}
			onTimesheetStatusChange={onTimesheetStatusChange}
			key={`${period[0].toISOString()}${period.length}`}
			row={updatedRow}
		/>
	);
};

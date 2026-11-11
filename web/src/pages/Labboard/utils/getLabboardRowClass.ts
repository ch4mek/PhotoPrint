import { LABBOARD_ROW_CLASSES } from '../constants';
import { LabboardGridRow } from '../types';

import { EMPTY_GRID_CELL_VALUE } from '@/components/Grid/constants';
import { DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';
import { InternshipFinalResult } from '@/types/labboard';

const hasConclusiveFinalResult = (finalResult: InternshipFinalResult | null) =>
	finalResult === InternshipFinalResult.Success ||
	finalResult === InternshipFinalResult.Fired;

const isLabboardPlannedEndOverdue = (
	plannedEndDate: string,
	finalResult: InternshipFinalResult | null
) => {
	if (
		hasConclusiveFinalResult(finalResult) ||
		plannedEndDate === EMPTY_GRID_CELL_VALUE
	) {
		return false;
	}

	const parsedDate = dayjs(plannedEndDate, DATE_FORMAT, true);

	if (!parsedDate.isValid()) {
		return false;
	}

	return parsedDate.startOf('day').isBefore(dayjs().startOf('day'));
};

export const getLabboardRowClass = (row: LabboardGridRow) => {
	const classes: string[] = [];

	if (row.internshipFinalResult === InternshipFinalResult.Success) {
		classes.push(LABBOARD_ROW_CLASSES.finalResultSuccess);
	}

	if (row.internshipFinalResult === InternshipFinalResult.Fired) {
		classes.push(LABBOARD_ROW_CLASSES.finalResultFired);
	}

	if (
		isLabboardPlannedEndOverdue(
			row.internshipPlannedEndDate,
			row.internshipFinalResult
		)
	) {
		classes.push(LABBOARD_ROW_CLASSES.plannedEndOverdue);
	}

	if (!row.isActive) {
		classes.push(LABBOARD_ROW_CLASSES.inactiveUser);
	}

	return classes.join(' ');
};

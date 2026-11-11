import { LabboardGridRow } from '../types';

import { HandleGridItemClick } from '@/components/Grid';
import { EMPTY_GRID_CELL_VALUE } from '@/components/Grid/constants';
import { formatDateString } from '@/helpers/dateToISOString';
import { InternshipFinalResult, LabboardIntern } from '@/types/labboard';

const mapFinalResultToCellValue = (
	finalResult: InternshipFinalResult | null
) => {
	if (
		finalResult === InternshipFinalResult.Success ||
		finalResult === InternshipFinalResult.Fired
	) {
		return finalResult;
	}

	return EMPTY_GRID_CELL_VALUE;
};

export const mapLabboardInternToRow = (
	{
		id,
		fullName,
		internship,
		resourceGroupName,
		englishLevelName,
		currentLocation,
		isActive,
		comments,
	}: LabboardIntern,
	handleEdit?: HandleGridItemClick
): LabboardGridRow => ({
	id,
	fullName: fullName ?? EMPTY_GRID_CELL_VALUE,
	internshipSpecialization: internship?.specialization ?? EMPTY_GRID_CELL_VALUE,
	resourceGroupName: resourceGroupName ?? EMPTY_GRID_CELL_VALUE,
	internshipTypeOrderWeight: internship?.type ?? EMPTY_GRID_CELL_VALUE,
	internshipEngineerStage:
		internship == null
			? EMPTY_GRID_CELL_VALUE
			: String(internship.engineerStage),
	englishLevelOrderWeight: englishLevelName ?? EMPTY_GRID_CELL_VALUE,
	internshipApprovedPoints:
		internship?.approvedPoints == null
			? EMPTY_GRID_CELL_VALUE
			: String(internship.approvedPoints),
	internshipCompletedPoints:
		internship?.completedPoints == null
			? EMPTY_GRID_CELL_VALUE
			: String(internship.completedPoints),
	currentLocation: currentLocation ?? EMPTY_GRID_CELL_VALUE,
	internshipStartDate: formatDateString(
		internship?.startDate ?? null,
		EMPTY_GRID_CELL_VALUE
	),
	internshipPlannedEndDate: formatDateString(
		internship?.plannedEndDate ?? null,
		EMPTY_GRID_CELL_VALUE
	),
	isActive,
	internshipFinalResultOrderWeight: mapFinalResultToCellValue(
		internship?.finalResult ?? null
	),
	comments: comments ?? EMPTY_GRID_CELL_VALUE,
	internshipFinalResult: internship?.finalResult ?? null,
	disabled: !isActive,
	handleEditClick: isActive ? handleEdit : undefined,
});

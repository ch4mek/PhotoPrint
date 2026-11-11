import { initialLabboardFilters } from '../constants';
import { LabboardFilters } from '../types';

import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
} from '@/helpers/areValuesEqual';
import { Booleanify } from '@/types/helpers';

export const compareLabboardFilters = (
	filters: LabboardFilters
): Booleanify<LabboardFilters> => ({
	id: filters.id === initialLabboardFilters.id,
	user: filters.user === initialLabboardFilters.user,
	internshipSpecialization:
		filters.internshipSpecialization ===
		initialLabboardFilters.internshipSpecialization,
	group: filters.group === initialLabboardFilters.group,
	internshipType: areArraysEqualByPrimitiveItems(
		filters.internshipType,
		initialLabboardFilters.internshipType
	),
	internshipEngineerStage:
		filters.internshipEngineerStage ===
		initialLabboardFilters.internshipEngineerStage,
	englishLevel: areArraysEqualByPrimitiveItems(
		filters.englishLevel,
		initialLabboardFilters.englishLevel
	),
	internshipApprovedPoints: areFromToCompatibleObjectsEqual(
		filters.internshipApprovedPoints,
		initialLabboardFilters.internshipApprovedPoints
	),
	internshipCompletedPoints: areFromToCompatibleObjectsEqual(
		filters.internshipCompletedPoints,
		initialLabboardFilters.internshipCompletedPoints
	),
	currentLocation:
		filters.currentLocation === initialLabboardFilters.currentLocation,
	internshipStartDate: areFromToCompatibleObjectsEqual(
		filters.internshipStartDate,
		initialLabboardFilters.internshipStartDate
	),
	internshipPlannedEndDate: areFromToCompatibleObjectsEqual(
		filters.internshipPlannedEndDate,
		initialLabboardFilters.internshipPlannedEndDate
	),
	status: areArraysEqualByPrimitiveItems(
		filters.status,
		initialLabboardFilters.status
	),
	internshipFinalResult: areArraysEqualByPrimitiveItems(
		filters.internshipFinalResult,
		initialLabboardFilters.internshipFinalResult
	),
	comments: filters.comments === initialLabboardFilters.comments,
});

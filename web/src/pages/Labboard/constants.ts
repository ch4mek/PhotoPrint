import { defaultIdColumnWidth } from '@/components/Grid/constants';
import {
	fromToCurrencyObject,
	fromToDateObject,
} from '@/constants/defaultValues';
import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { convertConstantToSelectionModalOption } from '@/helpers/convertToSelectionModalOptions';
import { IS_ACTIVE_DEFAULT_VALUE } from '@/pages/constants';
import { GridColumnWidthsConfiguration } from '@/types/grid';
import { InternshipFinalResult, InternshipType } from '@/types/labboard';

import { LabboardFilters } from './types';

export const LABBOARD_DEFAULT_ORDER_BY = 'id';

export const LABBOARD_INTERNSHIP_TYPE_OPTIONS = Object.values(
	InternshipType
).map(convertConstantToSelectionModalOption);

export const LABBOARD_FINAL_RESULT_OPTIONS = [
	...Object.values(InternshipFinalResult).map(
		convertConstantToSelectionModalOption
	),
	{
		id: NO_VALUE_FILTER_OPTION.id,
		description: NO_VALUE_FILTER_OPTION.description,
	},
];

export const initialLabboardFilters: LabboardFilters = {
	id: '',
	user: '',
	internshipSpecialization: '',
	group: '',
	internshipType: [],
	internshipEngineerStage: '',
	englishLevel: [],
	internshipApprovedPoints: fromToCurrencyObject,
	internshipCompletedPoints: fromToCurrencyObject,
	currentLocation: '',
	internshipStartDate: fromToDateObject,
	internshipPlannedEndDate: fromToDateObject,
	status: IS_ACTIVE_DEFAULT_VALUE,
	internshipFinalResult: [],
	comments: '',
};

export const labboardGridColumns = {
	id: {
		key: 'id',
		name: 'Id',
	},
	user: {
		key: 'fullName',
		name: 'User',
	},
	internshipSpecialization: {
		key: 'internshipSpecialization',
		name: 'Internship specialization',
	},
	group: {
		key: 'resourceGroupName',
		name: 'Group',
	},
	internshipType: {
		key: 'internshipTypeOrderWeight',
		name: 'Type',
	},
	internshipEngineerStage: {
		key: 'internshipEngineerStage',
		name: 'Engineer stage',
	},
	englishLevel: {
		key: 'englishLevelOrderWeight',
		name: 'Eng. lvl.',
	},
	internshipCompletedPoints: {
		key: 'internshipCompletedPoints',
		name: 'Compl. asses.',
	},
	internshipApprovedPoints: {
		key: 'internshipApprovedPoints',
		name: 'Appr. asses.',
	},
	currentLocation: {
		key: 'currentLocation',
		name: 'Current location',
	},
	internshipStartDate: {
		key: 'internshipStartDate',
		name: 'Start date',
	},
	internshipPlannedEndDate: {
		key: 'internshipPlannedEndDate',
		name: 'Planned end date',
	},
	status: {
		key: 'isActive',
		name: 'Status',
	},
	internshipFinalResult: {
		key: 'internshipFinalResultOrderWeight',
		name: 'Final result',
	},
	comments: {
		key: 'comments',
		name: 'Comment',
	},
} as const;

export const labboardOrderByToODataPath: Record<string, string> = {
	[labboardGridColumns.id.key]: 'id',
	[labboardGridColumns.user.key]: 'fullName',
	[labboardGridColumns.internshipSpecialization.key]:
		'internship/specialization',
	[labboardGridColumns.group.key]: 'resourceGroupName',
	[labboardGridColumns.internshipType.key]: 'internship/typeOrderWeight',
	[labboardGridColumns.internshipEngineerStage.key]: 'internship/engineerStage',
	[labboardGridColumns.englishLevel.key]: 'englishLevelOrderWeight',
	[labboardGridColumns.internshipCompletedPoints.key]:
		'internship/completedPoints',
	[labboardGridColumns.internshipApprovedPoints.key]:
		'internship/approvedPoints',
	[labboardGridColumns.currentLocation.key]: 'currentLocation',
	[labboardGridColumns.internshipStartDate.key]: 'internship/startDate',
	[labboardGridColumns.internshipPlannedEndDate.key]:
		'internship/plannedEndDate',
	[labboardGridColumns.status.key]: 'isActive',
	[labboardGridColumns.internshipFinalResult.key]:
		'internship/finalResultOrderWeight',
	[labboardGridColumns.comments.key]: 'comments',
};

export const getLabboardColumnsWidths = (showGroup: boolean) =>
	({
		id: defaultIdColumnWidth,
		user: { width: 200 },
		internshipSpecialization: { width: 200 },
		...(showGroup && {
			group: { width: 100 },
		}),
		internshipType: { width: 80, fixed: true },
		internshipEngineerStage: { width: 134, fixed: true },
		englishLevel: { width: 94, fixed: true },
		internshipCompletedPoints: { width: 128, fixed: true },
		internshipApprovedPoints: { width: 118, fixed: true },
		currentLocation: { width: 142 },
		internshipStartDate: { width: 110, fixed: true },
		internshipPlannedEndDate: { width: 147, fixed: true },
		status: { width: 88, fixed: true },
		internshipFinalResult: { width: 112, fixed: true },
		comments: { width: 200 },
	} satisfies GridColumnWidthsConfiguration);

export const getLabboardColumnsAmount = (showGroup: boolean) =>
	Object.keys(getLabboardColumnsWidths(showGroup)).length;

export const LABBOARD_ROW_CLASSES = {
	inactiveUser: 'inactive-user-row',
	plannedEndOverdue: 'planned-end-overdue',
	finalResultSuccess: 'final-result-success',
	finalResultFired: 'final-result-fired',
} as const;

export const LABBOARD_PLANNED_END_CELL_CLASS = 'labboard-planned-end-cell';

export const LABBOARD_COLOR_MIX_WHITE_PERCENTAGE = 90;

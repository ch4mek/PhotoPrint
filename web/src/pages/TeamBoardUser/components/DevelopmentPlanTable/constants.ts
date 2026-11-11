import {
	DevelopmentPlanSectionTableColumns,
	DevelopmentPlanTabTableColumns,
} from '@/types/developmentPlan';
import {
	GridColumnsNamesConfiguration,
	GridColumnWidthsConfiguration,
} from '@/types/grid';
import { TeamboardTableMode } from '@/types/teamboard';

import { DevelopmentPlanFormSectionTableColumns } from './types';

const developmentPlanTabTableColumnsWidths: GridColumnWidthsConfiguration<DevelopmentPlanTabTableColumns> =
	{
		status: {
			width: 115,
			fixed: true,
		},
		deadlineDate: {
			width: 150,
			fixed: true,
		},
		summary: 300,
		notes: 300,
		startDate: {
			width: 115,
			fixed: true,
		},
		endDate: {
			width: 150,
			fixed: true,
		},
	};

const developmentPlanSectionTableColumnsWidths: GridColumnWidthsConfiguration<DevelopmentPlanSectionTableColumns> =
	{
		status: 115,
		deadlineDate: 100,
		summary: 300,
	};

const developmentPlanFormSectionTableColumnsWidths: GridColumnWidthsConfiguration<DevelopmentPlanFormSectionTableColumns> =
	{
		startDate: {
			width: 100,
			fixed: true,
		},
		deadlineDate: {
			width: 100,
			fixed: true,
		},
		summary: 300,
	};

export const developmentPlanTableColumnsWidths: Record<
	TeamboardTableMode,
	Partial<GridColumnWidthsConfiguration<DevelopmentPlanTabTableColumns>>
> = {
	[TeamboardTableMode.PopUp]: developmentPlanFormSectionTableColumnsWidths,
	[TeamboardTableMode.Section]: developmentPlanSectionTableColumnsWidths,
	[TeamboardTableMode.Tab]: developmentPlanTabTableColumnsWidths,
	[TeamboardTableMode.Unknown]: developmentPlanTabTableColumnsWidths,
};

export const developmentPlanTableColumnsNames: GridColumnsNamesConfiguration<DevelopmentPlanTabTableColumns> =
	{
		status: {
			key: 'status',
			name: 'Status',
		},
		deadlineDate: {
			key: 'deadlineDate',
			name: 'Deadline date',
		},
		summary: {
			key: 'summary',
			name: 'Summary',
		},
		notes: {
			key: 'notes',
			name: 'Notes',
		},
		startDate: {
			key: 'startDate',
			name: 'Start date',
		},
		endDate: {
			key: 'endDate',
			name: 'Confirm. date',
		},
	};

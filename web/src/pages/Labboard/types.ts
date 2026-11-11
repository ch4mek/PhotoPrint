import {
	FromToProps,
	GridCellDefaultProps,
	HandleGridItemClick,
} from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { SetFilters } from '@/types/helpers';
import { InternshipFinalResult } from '@/types/labboard';

export interface LabboardFilters {
	id: string;
	user: string;
	internshipSpecialization: string;
	group: string;
	internshipType: string[];
	internshipEngineerStage: string;
	englishLevel: string[];
	internshipApprovedPoints: FromToProps<string>;
	internshipCompletedPoints: FromToProps<string>;
	currentLocation: string;
	internshipStartDate: FromToProps<Date>;
	internshipPlannedEndDate: FromToProps<Date>;
	status: string[];
	internshipFinalResult: string[];
	comments: string;
}

export enum LabboardFiltersActionType {
	SetId = 'SetId',
	SetUser = 'SetUser',
	SetInternshipSpecialization = 'SetInternshipSpecialization',
	SetGroup = 'SetGroup',
	SetInternshipType = 'SetInternshipType',
	SetInternshipEngineerStage = 'SetInternshipEngineerStage',
	SetEnglishLevel = 'SetEnglishLevel',
	SetInternshipApprovedPoints = 'SetInternshipApprovedPoints',
	SetInternshipCompletedPoints = 'SetInternshipCompletedPoints',
	SetCurrentLocation = 'SetCurrentLocation',
	SetInternshipStartDate = 'SetInternshipStartDate',
	SetInternshipPlannedEndDate = 'SetInternshipPlannedEndDate',
	SetStatus = 'SetStatus',
	SetInternshipFinalResult = 'SetInternshipFinalResult',
	SetComments = 'SetComments',
	Reset = 'Reset',
	Set = 'Set',
}

export type LabboardFiltersAction =
	| { type: LabboardFiltersActionType.SetId; payload: string }
	| { type: LabboardFiltersActionType.SetUser; payload: string }
	| {
			type: LabboardFiltersActionType.SetInternshipSpecialization;
			payload: string;
	  }
	| { type: LabboardFiltersActionType.SetGroup; payload: string }
	| {
			type: LabboardFiltersActionType.SetInternshipType;
			payload: string[];
	  }
	| {
			type: LabboardFiltersActionType.SetInternshipEngineerStage;
			payload: string;
	  }
	| { type: LabboardFiltersActionType.SetEnglishLevel; payload: string[] }
	| {
			type: LabboardFiltersActionType.SetInternshipApprovedPoints;
			payload: FromToProps<string>;
	  }
	| {
			type: LabboardFiltersActionType.SetInternshipCompletedPoints;
			payload: FromToProps<string>;
	  }
	| { type: LabboardFiltersActionType.SetCurrentLocation; payload: string }
	| {
			type: LabboardFiltersActionType.SetInternshipStartDate;
			payload: FromToProps<Date>;
	  }
	| {
			type: LabboardFiltersActionType.SetInternshipPlannedEndDate;
			payload: FromToProps<Date>;
	  }
	| { type: LabboardFiltersActionType.SetStatus; payload: string[] }
	| {
			type: LabboardFiltersActionType.SetInternshipFinalResult;
			payload: string[];
	  }
	| { type: LabboardFiltersActionType.SetComments; payload: string }
	| { type: LabboardFiltersActionType.Set; payload: LabboardFilters }
	| { type: LabboardFiltersActionType.Reset };

export interface LabboardGridOptions {
	users: string[];
	groups: string[];
	englishLevels: SelectionModalOption[];
	internshipTypes: SelectionModalOption[];
	finalResults: SelectionModalOption[];
}

export interface UseLabboardColumnsProps {
	filters: LabboardFilters;
	setFilters: SetFilters<LabboardFilters>;
	options: LabboardGridOptions;
	isResetFiltersDisabled: boolean;
	handleUserClick?: HandleGridItemClick;
}

export interface LabboardGridRow extends GridCellDefaultProps {
	id: number;
	fullName: string;
	internshipSpecialization: string;
	resourceGroupName: string;
	internshipTypeOrderWeight: string;
	internshipEngineerStage: string;
	englishLevelOrderWeight: string;
	internshipApprovedPoints: string;
	internshipCompletedPoints: string;
	currentLocation: string;
	internshipStartDate: string;
	internshipPlannedEndDate: string;
	isActive: boolean;
	internshipFinalResultOrderWeight: string;
	comments: string;
	internshipFinalResult: InternshipFinalResult | null;
}

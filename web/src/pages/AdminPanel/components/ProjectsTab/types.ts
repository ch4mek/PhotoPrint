import { FromToProps } from '@/components/Grid';
import { SetFilters } from '@/types/helpers';

export interface ProjectsFilters {
	projectFilter: string;
	clientFilter: string;
	factTrackedTimeFilter: FromToProps<string>;
	billableTrackedTimeFilter: FromToProps<string>;
	statusFilter: string[];
	publicFilter: string[];
	billableFilter: string[];
	idFilter: string;
}

export interface ProjectsSetFilters extends SetFilters<ProjectsFilters> {}

export enum ProjectsFiltersActionType {
	SET_PROJECT_FILTER = 'SET_PROJECT_FILTER',
	SET_CLIENT_FILTER = 'SET_CLIENT_FILTER',
	SET_FACT_TRACKED_TIME_FILTER = 'SET_FACT_TRACKED_TIME_FILTER',
	SET_BILLABLE_TRACKED_TIME_FILTER = 'SET_BILLABLE_TRACKED_TIME_FILTER',
	SET_STATUS_FILTER = 'SET_STATUS_FILTER',
	SET_PUBLIC_FILTER = 'SET_PUBLIC_FILTER',
	SET_BILLABLE_FILTER = 'SET_BILLABLE_FILTER',
	SET_ID_FILTER = 'SET_ID_FILTER',
	RESET = 'RESET',
	SET = 'SET',
}

export type ProjectsFiltersAction =
	| { type: ProjectsFiltersActionType.SET_PROJECT_FILTER; payload: string }
	| { type: ProjectsFiltersActionType.SET_CLIENT_FILTER; payload: string }
	| {
			type: ProjectsFiltersActionType.SET_FACT_TRACKED_TIME_FILTER;
			payload: FromToProps<string>;
	  }
	| {
			type: ProjectsFiltersActionType.SET_BILLABLE_TRACKED_TIME_FILTER;
			payload: FromToProps<string>;
	  }
	| { type: ProjectsFiltersActionType.SET_STATUS_FILTER; payload: string[] }
	| { type: ProjectsFiltersActionType.SET_PUBLIC_FILTER; payload: string[] }
	| { type: ProjectsFiltersActionType.SET_BILLABLE_FILTER; payload: string[] }
	| { type: ProjectsFiltersActionType.SET_ID_FILTER; payload: string }
	| { type: ProjectsFiltersActionType.SET; payload: ProjectsFilters }
	| { type: ProjectsFiltersActionType.RESET };

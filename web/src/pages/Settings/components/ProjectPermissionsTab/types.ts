export interface ProjectPermissionsFilters {
	userIdFilter: string;
	userFullNameFilter: string;
	projectNameFilter: string;
}

type ProjectPermissionsSetFilterFn = (payload: string) => void;

export interface ProjectPermissionsSetFilters {
	userIdFilter: ProjectPermissionsSetFilterFn;
	userFullNameFilter: ProjectPermissionsSetFilterFn;
	projectNameFilter: ProjectPermissionsSetFilterFn;
	applyFilters: (payload: ProjectPermissionsFilters) => void;
	resetFilters: VoidFunction;
}

export enum ProjectPermissionsFilterActionType {
	SET_USER_ID_FILTER = 'SET_USER_ID_FILTER',
	SET_USER_FULLNAME_FILTER = 'SET_USER_FULLNAME_FILTER',
	SET_PROJECT_NAME_FILTER = 'SET_PROJECT_NAME_FILTER',
	RESET_FILTERS = 'RESET_FILTERS',
	APPLY_FILTERS = 'APPLY_FILTERS',
}

export type ProjectPermissionsFilterAction =
	| {
			type: ProjectPermissionsFilterActionType.SET_USER_ID_FILTER;
			payload: string;
	  }
	| {
			type: ProjectPermissionsFilterActionType.SET_USER_FULLNAME_FILTER;
			payload: string;
	  }
	| {
			type: ProjectPermissionsFilterActionType.SET_PROJECT_NAME_FILTER;
			payload: string;
	  }
	| { type: ProjectPermissionsFilterActionType.RESET_FILTERS }
	| {
			type: ProjectPermissionsFilterActionType.APPLY_FILTERS;
			payload: ProjectPermissionsFilters;
	  };

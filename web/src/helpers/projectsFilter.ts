import { FromToProps } from '@/components/Grid';
import {
	ProjectsTableFilters,
	ProjectAction,
	ProjectTableFiltersKeys,
	ProjectTableFilterSetter,
} from '@/types/projectsTableFilters';

export const projectTableFiltersReducer = (
	state: ProjectsTableFilters,
	{ type, payload }: ProjectAction
): ProjectsTableFilters => {
	switch (type) {
		case ProjectTableFiltersKeys.projectName: {
			return { ...state, projectName: payload as string };
		}

		case ProjectTableFiltersKeys.clientName: {
			return { ...state, clientName: payload as string };
		}

		case ProjectTableFiltersKeys.billability: {
			return { ...state, billability: payload as string[] };
		}

		case ProjectTableFiltersKeys.isActive: {
			return { ...state, isActive: payload as string[] };
		}

		case ProjectTableFiltersKeys.startDate: {
			return {
				...state,
				startDate: {
					...state.startDate,
					...(payload as FromToProps<Date>),
				},
			};
		}

		case ProjectTableFiltersKeys.endDate: {
			return {
				...state,
				endDate: {
					...state.endDate,
					...(payload as FromToProps<Date>),
				},
			};
		}

		case ProjectTableFiltersKeys.set: {
			return payload as ProjectsTableFilters;
		}

		default: {
			return state;
		}
	}
};

export const generateSetFilters = (
	dispatch: React.Dispatch<ProjectAction>
): ProjectTableFilterSetter => ({
	projectName: (newName: string) =>
		dispatch({ type: ProjectTableFiltersKeys.projectName, payload: newName }),
	clientName: (newClientName: string) =>
		dispatch({
			type: ProjectTableFiltersKeys.clientName,
			payload: newClientName,
		}),
	isActive: (isActive: string[]) =>
		dispatch({
			type: ProjectTableFiltersKeys.isActive,
			payload: isActive,
		}),
	billability: (billabilities: string[]) =>
		dispatch({
			type: ProjectTableFiltersKeys.billability,
			payload: billabilities,
		}),
	startDate: (payload: FromToProps<Date>) =>
		dispatch({ type: ProjectTableFiltersKeys.startDate, payload }),
	endDate: (payload: FromToProps<Date>) =>
		dispatch({ type: ProjectTableFiltersKeys.endDate, payload }),
	set: (newFilters: ProjectsTableFilters) =>
		dispatch({ type: ProjectTableFiltersKeys.set, payload: newFilters }),
});

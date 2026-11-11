import {
	SpecializationTableFilters,
	SpecializationTableFilterSetters,
} from './components/SpecializationTable/types';
import { DEFAULT_SPECIALIZATION_FILTER_VALUES } from './constants';
import { SpecializationAction, SpecializationTableFiltersKeys } from './types';

export const specializationTableFiltersReducer = (
	state: SpecializationTableFilters,
	{ type, payload }: SpecializationAction
): SpecializationTableFilters => {
	switch (type) {
		case SpecializationTableFiltersKeys.SpecializationName: {
			return { ...state, specializationName: payload as string };
		}

		case SpecializationTableFiltersKeys.Set: {
			return payload as SpecializationTableFilters;
		}

		case SpecializationTableFiltersKeys.Reset: {
			return payload as SpecializationTableFilters;
		}

		default: {
			return state;
		}
	}
};

export const generateSetFilters = (
	dispatch: React.Dispatch<SpecializationAction>
): SpecializationTableFilterSetters => ({
	specializationName: (newName: string) =>
		dispatch({
			type: SpecializationTableFiltersKeys.SpecializationName,
			payload: newName,
		}),
	set: (newFilters: SpecializationTableFilters) =>
		dispatch({ type: SpecializationTableFiltersKeys.Set, payload: newFilters }),
	reset: () =>
		dispatch({
			type: SpecializationTableFiltersKeys.Reset,
			payload: DEFAULT_SPECIALIZATION_FILTER_VALUES,
		}),
});

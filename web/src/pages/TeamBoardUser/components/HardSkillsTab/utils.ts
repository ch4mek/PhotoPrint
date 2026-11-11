import {
	HardSkillRow,
	SpecializationAction,
	SpecializationTableFiltersKeys,
} from '../../types';
import {
	SpecializationTableFilters,
	SpecializationTableFilterSetters,
} from '../SpecializationTable/types';

import { DEFAULT_SPECIALIZATION_FILTER_VALUES } from './constants';

export const getLoaderRows = (): Array<HardSkillRow> => [
	{
		id: 0,
		showSkeleton: true,
		isPrimary: false,
		seniority: '',
		name: '',
	},
];

export const specializationTableFiltersReducer = (
	state: SpecializationTableFilters,
	{ type, payload }: SpecializationAction
): SpecializationTableFilters => {
	switch (type) {
		case SpecializationTableFiltersKeys.specializationName: {
			return { ...state, specializationName: payload as string };
		}

		case SpecializationTableFiltersKeys.set: {
			return payload as SpecializationTableFilters;
		}

		case SpecializationTableFiltersKeys.reset: {
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
			type: SpecializationTableFiltersKeys.specializationName,
			payload: newName,
		}),
	set: (newFilters: SpecializationTableFilters) =>
		dispatch({ type: SpecializationTableFiltersKeys.set, payload: newFilters }),
	reset: () =>
		dispatch({
			type: SpecializationTableFiltersKeys.reset,
			payload: DEFAULT_SPECIALIZATION_FILTER_VALUES,
		}),
});

import { Dispatch } from 'react';

import { DEFAULT_SOFT_SKILLS_FILTER_VALUES } from '@/pages/TeamBoardUser/components/SoftSkillsTab/constants';
import {
	SoftSkillAction,
	SoftSkillsFilterSetter,
	SoftSkillTableFilters,
	SoftSkillTableFiltersKeys,
} from '@/types/softSkills';

export const softSkillsFiltersReducer = (
	state: SoftSkillTableFilters,
	{ type, payload }: SoftSkillAction
): SoftSkillTableFilters => {
	switch (type) {
		case SoftSkillTableFiltersKeys.softSkillName: {
			return { ...state, softSkillName: payload as string };
		}

		case SoftSkillTableFiltersKeys.set: {
			return payload as SoftSkillTableFilters;
		}

		case SoftSkillTableFiltersKeys.reset: {
			return payload as SoftSkillTableFilters;
		}

		default: {
			return state;
		}
	}
};

export const generateSetFilters = (
	dispatch: Dispatch<SoftSkillAction>
): SoftSkillsFilterSetter => ({
	softSkillName: (newName: string) =>
		dispatch({
			type: SoftSkillTableFiltersKeys.softSkillName,
			payload: newName,
		}),
	set: (newFilters: SoftSkillTableFilters) =>
		dispatch({ type: SoftSkillTableFiltersKeys.set, payload: newFilters }),
	reset: () =>
		dispatch({
			type: SoftSkillTableFiltersKeys.reset,
			payload: DEFAULT_SOFT_SKILLS_FILTER_VALUES,
		}),
});

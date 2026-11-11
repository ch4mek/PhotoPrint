import { arePrimitiveValuesEqual } from '@/helpers/areValuesEqual';
import { DEFAULT_SOFT_SKILLS_FILTER_VALUES } from '@/pages/TeamBoardUser/components/SoftSkillsTab/constants';
import { SoftSkillTableFilters } from '@/types/softSkills';

export const isResetDisabled = (filters: SoftSkillTableFilters) =>
	(Object.keys(filters) as Array<keyof typeof filters>).some((key) =>
		arePrimitiveValuesEqual(
			filters[key],
			DEFAULT_SOFT_SKILLS_FILTER_VALUES[key]
		)
	);

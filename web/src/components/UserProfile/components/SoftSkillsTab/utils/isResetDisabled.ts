import { DEFAULT_SOFT_SKILLS_FILTER_VALUES } from '@/components/UserProfile/components/SoftSkillsTab/constants';
import { arePrimitiveValuesEqual } from '@/helpers/areValuesEqual';
import { SoftSkillTableFilters } from '@/types/softSkills';

export const isResetDisabled = (filters: SoftSkillTableFilters) =>
	(Object.keys(filters) as Array<keyof typeof filters>).some((key) =>
		arePrimitiveValuesEqual(
			filters[key],
			DEFAULT_SOFT_SKILLS_FILTER_VALUES[key]
		)
	);

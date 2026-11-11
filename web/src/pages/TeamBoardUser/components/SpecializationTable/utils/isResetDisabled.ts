import { DEFAULT_SPECIALIZATION_FILTER_VALUES } from '../../HardSkillsTab/constants';
import { SpecializationTableFilters } from '../types';

import { arePrimitiveValuesEqual } from '@/helpers/areValuesEqual';

export const isResetDisabled = (filters: SpecializationTableFilters) =>
	(Object.keys(filters) as Array<keyof typeof filters>).some((key) =>
		arePrimitiveValuesEqual(
			filters[key],
			DEFAULT_SPECIALIZATION_FILTER_VALUES[key]
		)
	);

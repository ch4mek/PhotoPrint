import { DEFAULT_EDUCATIONS_FILTER_VALUES } from '../constants';

import { EducationsGridFilters } from '@/types/educations';

export const checkForEmptyFilter = (filters: EducationsGridFilters) =>
	JSON.stringify(filters) === JSON.stringify(DEFAULT_EDUCATIONS_FILTER_VALUES);

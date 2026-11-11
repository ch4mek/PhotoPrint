import { defaultJiraSettingsFilters } from '../constants';
import {
	generateJiraSettingsSetFilters,
	jiraSettingsFiltersComparator,
	jiraSettingsFiltersReducer,
} from '../utils';

import { useFilters } from '@/hooks/useFilters';

export const useJiraSettingsFilters = (
	initialFilters = defaultJiraSettingsFilters
) =>
	useFilters(
		defaultJiraSettingsFilters,
		jiraSettingsFiltersReducer,
		generateJiraSettingsSetFilters,
		initialFilters,
		jiraSettingsFiltersComparator
	);

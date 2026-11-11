import { Dayjs } from '@/lib/dayjs';
import { HandleRowClick } from '@/types/grid';
import { SetFilters } from '@/types/helpers';
import {
	JiraSettingsFilters,
	JiraSettingsEntity,
	JiraSettingsGridRow,
} from '@/types/jira';

export enum JiraSettingsFiltersNames {
	URL = 'url',
	EMAIL = 'email',
	PROJECT_NAME = 'projectName',
	PROJECT_TAG = 'projectTag',
	RESET = 'reset',
	SET = 'set',
}

export type JiraSettingsAction =
	| {
			[K in keyof JiraSettingsFilters]: {
				type: K;
				payload: JiraSettingsFilters[K];
			};
	  }[keyof JiraSettingsFilters]
	| { type: JiraSettingsFiltersNames.RESET }
	| { type: JiraSettingsFiltersNames.SET; payload: JiraSettingsFilters };

export interface JiraSettingsSetFilters
	extends SetFilters<JiraSettingsFilters> {}

export interface UseJiraSettingsRowsProps {
	items?: Array<JiraSettingsEntity>;
	dataUpdatedAt: Dayjs | Date | number;
	page: number;
	rowsPerPage: number;
}

export interface UseJiraSettingsColumnsProps {
	filters: JiraSettingsFilters;
	setFilters: JiraSettingsSetFilters;
	isResetFiltersDisabled: boolean;
	handleDelete: HandleRowClick<JiraSettingsGridRow>;
	handleEdit: HandleRowClick<JiraSettingsGridRow>;
}

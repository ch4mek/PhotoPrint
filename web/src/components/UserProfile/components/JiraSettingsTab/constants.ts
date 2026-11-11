import { GridColumnWidthsConfiguration } from '@/types/grid';
import { JiraSettingsFilters } from '@/types/jira';

export const ADD_BUTTON_TITLE = 'Add setting';

export const DEFAULT_JIRA_SETTINGS_ORDER_BY_FIELD = 'id';

export const JIRA_SETTINGS_PAT_MASK_LENGTH = 40;

export const defaultJiraSettingsFilters = {
	url: '',
	email: '',
	projectName: '',
	projectTag: '',
} satisfies JiraSettingsFilters;

export const jiraSettingsGridColumns = {
	url: {
		key: 'url',
		name: 'Jira URL',
	},
	email: {
		key: 'email',
		name: 'Jira email',
		sortable: true,
	},
	personalAccessToken: {
		key: 'pat',
		name: 'PAT',
	},
	projectTag: {
		key: 'projectTag',
		name: 'Jira project tag',
		sortable: true,
	},
	projectName: {
		key: 'projectName',
		name: 'Project',
		sortable: true,
	},
};

export const jiraSettingsColumnsWidths = {
	url: 233,
	email: 233,
	personalAccessToken: 233,
	projectTag: { width: 140, maxWidth: 200 },
	projectName: 233,
} satisfies GridColumnWidthsConfiguration<typeof jiraSettingsGridColumns>;

export const JIRA_SETTINGS_EDIT_COLUMN_WIDTH = 44;

export const jiraSettingsMenuOptionsValues = {
	EDIT: 'Edit',
	DELETE: 'Delete',
} as const;

export const DELETE_DIALOG_TITLE = 'Jira setting';
export const DELETE_DIALOG_CONTENT =
	'Are you sure you want to delete this Jira setting?';

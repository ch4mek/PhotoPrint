import { JiraSettingsFormFields } from './type';

export const jiraSettingsFormFields: JiraSettingsFormFields = {
	url: {
		name: 'url',
		label: 'Jira URL',
	},
	email: {
		name: 'email',
		label: 'Jira email',
		withTrimOnBlur: true,
	},
	projectTag: {
		name: 'projectTag',
		label: 'Jira project tag',
	},
	project: {
		name: 'project',
		label: 'Project',
		valueField: 'name',
	},
	personalAccessToken: {
		name: 'personalAccessToken',
		label: 'PAT',
	},
};

export const DIVIDER_COLOR = 'graphics-input-stroke';

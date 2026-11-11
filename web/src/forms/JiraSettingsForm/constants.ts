import { JiraSettingsFormValues } from './types';

export const jiraSettingsFormInitialValues: JiraSettingsFormValues = {
	url: '',
	email: '',
	projectTag: '',
	project: null,
	personalAccessToken: '',
};

export const FORM_NAME = 'Jira setting';

const PAT_MASK_LENGTH = 40;
export const MASKED_PAT_PLACEHOLDER = '*'.repeat(PAT_MASK_LENGTH);

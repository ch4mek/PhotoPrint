import {
	JiraSettingsEntity,
	CreateJiraSettingsParams,
	UpdateJiraSettingsParams,
} from '@/types/jira';

import {
	JiraSettingsFormValues,
	ValidatedJiraSettingsFormValues,
} from './types';

export const getInitialValues = ({
	url,
	email,
	jiraProjectTag,
	projectId,
	projectName,
}: JiraSettingsEntity): JiraSettingsFormValues => ({
	url,
	email,
	projectTag: jiraProjectTag,
	project: {
		id: projectId,
		name: projectName ?? '',
	},
	personalAccessToken: '',
});

export const mapJiraSettingsFormValuesToCreateParams = (
	{
		url,
		email,
		projectTag,
		personalAccessToken,
		project,
	}: ValidatedJiraSettingsFormValues,
	userId: number
): CreateJiraSettingsParams => ({
	userId,
	url,
	email,
	jiraProjectTag: projectTag,
	pat: personalAccessToken,
	projectId: project.id,
});

export const mapJiraSettingsFormValuesToUpdateParams = (
	{
		url,
		email,
		projectTag,
		personalAccessToken,
		project,
	}: ValidatedJiraSettingsFormValues,
	userId: number,
	id: number
): UpdateJiraSettingsParams => ({
	id,
	userId,
	url,
	email,
	jiraProjectTag: projectTag,
	projectId: project.id,
	pat: personalAccessToken || undefined,
});

import { FormAutoselectOption } from '@/types/forms';
import { JiraSettingsEntity } from '@/types/jira';

export interface JiraSettingsFormProps {
	onClose: VoidFunction;
	jiraSettings?: JiraSettingsEntity;
}

export interface JiraSettingsFormValues {
	url: string;
	email: string;
	projectTag: string;
	project: FormAutoselectOption | null;
	personalAccessToken: string;
}

export type ValidatedJiraSettingsFormValues = {
	[Key in keyof JiraSettingsFormValues]: NonNullable<
		JiraSettingsFormValues[Key]
	>;
};

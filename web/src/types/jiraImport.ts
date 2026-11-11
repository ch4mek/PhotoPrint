export interface JiraImportTimesheetsPayload {
	startDate: string;
	endDate: string;
	projectId: number;
}

interface JiraImportProject {
	jiraProjectTag: string;
	jiraSettingId: number;
	projectId: number;
	projectName: string;
}

export type JiraImportProjectsResponse = JiraImportProject[];

export interface JiraImportProjectOption {
	id: number;
	name: string;
	jiraProjectTag: string;
}

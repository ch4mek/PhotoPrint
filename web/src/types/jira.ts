import { GridCellDefaultProps } from '@/components/Grid';

import { OdataParams } from './common';

export interface JiraSettingsFilters {
	url: string;
	email: string;
	projectTag: string;
	projectName: string;
}

export interface GetJiraSettingsQueryParams
	extends OdataParams<JiraSettingsFilters> {
	userId: number | undefined;
}

export interface JiraSettingsEntity {
	id: number;
	url: string;
	email: string;
	jiraProjectTag: string;
	projectId: number;
	userId: number;
	projectName: string | null;
}

type JiraSettingsPayloadBase = Pick<
	JiraSettingsEntity,
	'userId' | 'url' | 'email' | 'jiraProjectTag' | 'projectId'
>;

export interface CreateJiraSettingsParams extends JiraSettingsPayloadBase {
	pat: string;
}

export interface UpdateJiraSettingsParams extends JiraSettingsPayloadBase {
	id: JiraSettingsEntity['id'];
	pat?: string;
}

export interface JiraSettingsGridRow extends GridCellDefaultProps {
	id: number;
	url: string;
	email: string;
	projectTag: string;
	projectName: string;
	pat: string;
}
